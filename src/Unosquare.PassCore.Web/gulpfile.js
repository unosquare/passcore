/// <binding BeforeBuild='clean, min' Clean='clean, min' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    del = require("del"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    projectBase: "./",
};

paths.webRoot = paths.projectBase + "wwwroot/";
paths.bowerBase = paths.projectBase + "Bower/";

paths.bowerLibraries = {
    "jquery": paths.bowerBase + "jquery/dist/jquery*.{js,map}",
    "bootstrap": paths.bowerBase + "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
    "angular": paths.bowerBase + "angular/angular*.{js,map}",
    "angular-bootstrap": paths.bowerBase + "angular-bootstrap/ui-bootstrap*.{js,map,css}",
    "font-awesome": paths.bowerBase + "Font-Awesome/**/*.{css,otf,eot,svg,ttf,woff,wof2}"
};

paths.appScriptsBase = paths.webRoot + "scripts/";
paths.appStylesBase = paths.webRoot + "styles/";
paths.libBase = paths.webRoot + "lib/";

paths.appScriptsWildcard = paths.appScriptsBase + "**/*.js";
paths.appScriptsMinifiedWildcard = paths.appScriptsBase + "**/*.min.js";

paths.appStylesWildcard = paths.appStylesBase + "**/*.css";
paths.appStylesMinifiedWildcard = paths.appStylesBase + "**/*.min.css";

paths.appTargetScript = paths.appScriptsBase + "app.min.js";
paths.libTargetScript = paths.appScriptsBase + "lib.min.js";
paths.appTargetStyle = paths.appStylesBase + "app.min.css"


gulp.task("undeploy:lib", function (cb) {
    console.log("Removing Folder: " + paths.libBase);
    del.sync(paths.libBase);
    return cb();
});

gulp.task("copy:lib", ["undeploy:lib"], function (cb) {
    console.log('Copying Bower Libraries . . .');
    var lastSrc = null;
    for (var destinationDir in paths.bowerLibraries) {
        lastSrc = gulp.src(paths.bowerLibraries[destinationDir])
            .pipe(gulp.dest(paths.libBase + destinationDir));
        console.log("    Copied Library  '" + destinationDir + "'");
    };

    return cb();
});

gulp.task("clean:lib", ["copy:lib"], function (cb) {
    console.log('Removing Minified Files . . .');

    var targets = new Array();
    var index = 0;
    for (var destinationDir in paths.bowerLibraries) {
        targets[targets.length] = paths.libBase + destinationDir + "/**/*.min.js";
        targets[targets.length] = paths.libBase + destinationDir + "/**/*.min.js.map";
        targets[targets.length] = paths.libBase + destinationDir + "/**/*.min.css";
    }

    del.sync(targets);
    return cb();
});

gulp.task("build:lib", ["clean:lib"], function (cb) {
    return cb();
});

gulp.task("clean:scripts", function (cb) {
    rimraf(paths.appTargetScript, cb);
});

gulp.task("clean:styles", function (cb) {
    rimraf(paths.appTargetStyle, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.libJs, "!" + paths.minLibJs], { base: "." })
        .pipe(concat(paths.concatJsLibDest))
        .pipe(uglify())
        .pipe(gulp.dest(""));
});

gulp.task("min:lib", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest(""));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest(""));
});

gulp.task("min", ["min:js", "min:css"]);
