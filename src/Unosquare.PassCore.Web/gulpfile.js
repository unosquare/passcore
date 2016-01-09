/// <binding BeforeBuild='clean, min' Clean='clean, min' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/",
    bower: "./Bower/",
    lib: "./wwwroot/scripts/lib/"
};

paths.js = paths.webroot + "scripts/**/*.js";
paths.minJs = paths.webroot + "scripts/**/*.min.js";

paths.libJs = paths.webroot + "scripts/lib/**/*.js";
paths.minLibJs = paths.webroot + "scripts/lib/**/*.min.js";

paths.concatJsDest = paths.webroot + "scripts/app.min.js";
paths.concatJsLibDest = paths.webroot + "scripts/lib.min.js";

paths.css = paths.webroot + "styles/**/*.css";
paths.minCss = paths.webroot + "styles/**/*.min.css";

paths.concatCssDest = paths.webroot + "styles/app.min.css";


gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:lib", function (cb) {
    rimraf(paths.concatJsLibDest, cb);
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

gulp.task("copy", ["clean"], function () {
    var bower = {
        "angular": "angular/angular*.{js,map}",
        "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}",
        "jquery": "jquery/jquery*.{js,map}",
        "font-awesome": "Font-Awesome/**/*.{css,otf,eot,svg,ttf,woff,wof2}"
    };

    for (var destinationDir in bower) {
        gulp.src(paths.bower + bower[destinationDir])
                .pipe(gulp.dest(paths.lib + destinationDir));
    }
});