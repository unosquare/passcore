import * as webpack from 'webpack';
import * as path from 'path';

const config: webpack.Configuration = {
    entry: { foo:'./foo.js'},
    output: {
        path: '../wwwroot/scripts',
        filename: 'app.min.js'
    }
};

export default config;