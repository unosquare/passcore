/// <binding BeforeBuild='build:user' ProjectOpened='build:all' />
"use strict";

var gulp = require("gulp"),
    fs = require("fs"),
    rimraf = require("rimraf"),
    del = require("del"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    merge = require("merge-stream"),
    replace = require("gulp-replace"),
    flatten = require("gulp-flatten"),
    uglify = require("gulp-uglify");

var paths = {};

paths.projectBase = "./";
paths.webRoot = paths.projectBase + "wwwroot/";
paths.bowerBase = paths.projectBase + "bower_components/";

paths.appScriptsBase = paths.webRoot + "scripts/";
paths.appStylesBase = paths.webRoot + "styles/";
paths.libBase = paths.webRoot + "vendor/";

paths.appScriptsWildcard = paths.appScriptsBase + "**/*.js";
paths.appScriptsMinifiedWildcard = paths.appScriptsBase + "**/*.min.js";

paths.appStylesWildcard = paths.appStylesBase + "**/*.css";
paths.appStylesMinifiedWildcard = paths.appStylesBase + "**/*.min.css";

paths.appTargetScript = paths.appScriptsBase + "app.min.js";
paths.appTargetStyle = paths.appStylesBase + "app.min.css"

paths.libTargetScript = paths.libBase + "vendor.min.js";
paths.libTargetStyle = paths.libBase + "vendor.min.css";

paths.bowerLibraries = {
    "jquery": paths.bowerBase + "jquery/dist/jquery*.{js,map}",
    "bootstrap": paths.bowerBase + "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "angular": paths.bowerBase + "angular/angular*.{js,map}",
    "angular-bootstrap": paths.bowerBase + "angular-bootstrap/ui-bootstrap*.{js,map,css}",
    "angular-cookies": paths.bowerBase + "angular-cookies/**/*.{js,map,css}",
    "angular-route": paths.bowerBase + "angular-route/**/*.{js,map,css}",
    "angular-local-storage": paths.bowerBase + "angular-local-storage/**/*.{js,map,css}",
    "angular-recaptcha": paths.bowerBase + "angular-recaptcha/release/**/*.js",
    "toastr": paths.bowerBase + "toastr/**/*.{js,map,css}",
    "font-awesome": paths.bowerBase + "Font-Awesome/**/*.{css,otf,eot,svg,ttf,woff,wof2,woff2}"
};

// Removes the vendor folder completely
gulp.task("undeploy:vendor", function (cb) {
    console.log("Removing Folder: " + paths.libBase);
    del.sync(paths.libBase);
    return cb();
});

// Copies the relevant bower_component files to the vendor directory
gulp.task("copy:vendor", ["undeploy:vendor"], function (cb) {
    console.log('Copying Bower Libraries . . .');
    var streams = new Array();
    for (var destinationDir in paths.bowerLibraries) {
        streams[streams.length] = gulp.src(paths.bowerLibraries[destinationDir])
            .pipe(gulp.dest(paths.libBase + destinationDir));
        console.log("    Copied Library  '" + destinationDir + "'");
    };

    return merge(streams);
});

// Removes minified versions of files from the vendor folder
gulp.task("clean:vendor", ["copy:vendor"], function (cb) {
    console.log('Removing Minified Files . . .');

    var targets = [
        paths.libBase + "**/*.min.js",
        paths.libBase + "**/*.min.js.map",
        paths.libBase + "**/*.min.css",
        paths.libBase + "**/*.css.map",
        paths.libBase + "**/index.js",
        paths.libBase + 'bootstrap/js/npm.js',
        paths.libBase + 'files-saver-js/demo/demo.js',
        paths.libBase + '*.{js,css,otf,eot,svg,ttf,woff,wof2,woff2}'
    ];

    return del(targets);
});

// Minifies and flattens the vendor folder
gulp.task("build:vendor", ["clean:vendor"], function (cb) {
    var streams = new Array();

    var scriptTargets = new Array();
    var styleTargets = new Array();

    for (var destinationDir in paths.bowerLibraries) {
        // add the js files but not the min ones
        scriptTargets[scriptTargets.length] = paths.libBase + destinationDir + "/**/*.js";
        scriptTargets[scriptTargets.length] = "!" + paths.libBase + destinationDir + "/**/*min.js";

        // add the css files but not the min ones
        styleTargets[styleTargets.length] = paths.libBase + destinationDir + "/**/*.css";
        styleTargets[styleTargets.length] = "!" + paths.libBase + destinationDir + "/**/*min.css";
    };

    streams[streams.length] = gulp.src(scriptTargets)
        .pipe(uglify({
            outSourceMap: true,
            preserveComments: 'license'
        }))
        .pipe(concat(paths.libTargetScript))
        .pipe(gulp.dest(''));

    streams[streams.length] = gulp.src(styleTargets)
        .pipe(cssmin())
        .pipe(replace('../fonts/', ''))
        .pipe(concat(paths.libTargetStyle))
        .pipe(gulp.dest(''));

    var fontsFilter = paths.libBase + "**/*.{otf,eot,svg,ttf,woff,wof2,woff2}";
    streams[streams.length] = gulp.src(fontsFilter)
        .pipe(flatten())
        .pipe(gulp.dest(paths.libBase));

    return merge(streams);

});

// Removes the minifier version of the application script
gulp.task("clean:scripts", function (cb) {
    return del(paths.appTargetScript);
});

// Removes the minified version of the application styles
gulp.task("clean:styles", function (cb) {
    return del(paths.appTargetStyle);
});

// Removes minified versions from scripts, styles and vendor folders
gulp.task("clean", ["clean:scripts", "clean:styles", "clean:vendor"]);

// Builds a minified, concatenated version the application's javascript
gulp.task("build:scripts", ["clean:scripts"], function () {
    return gulp.src([paths.appScriptsWildcard, "!" + paths.appScriptsMinifiedWildcard], { base: "." })
        .pipe(concat(paths.appTargetScript))
        //.pipe(uglify()) // uncomment this line if you need to enable minification.
        .pipe(gulp.dest(""));
});

// Builds a minified, concatenated version of the application's css
gulp.task("build:styles", ["clean:styles"], function () {
    return gulp.src([paths.appStylesWildcard, "!" + paths.appStylesMinifiedWildcard])
        .pipe(concat(paths.appTargetStyle))
        .pipe(cssmin())
        .pipe(gulp.dest(""));
});

// Use this task to rebuild minified, concatenated versions of vendor, scripts and styles folder
gulp.task("build:all", ["build:scripts", "build:styles", "build:vendor"]);

// Use this task to only rebuild minified, concatenated versions of scripts and styles folders but excluding the vendor folder
gulp.task("build:user", ["build:scripts", "build:styles"]);