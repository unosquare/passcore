import * as webpack from 'webpack';
import * as path from 'path';

const config: webpack.Configuration = {
    entry: path.resolve('./wwwroot/app/app.module.ts'),
    devtool:'inline-source-map',
    module:{
        rules:
        [{
            test: /\.tsx?$/,
            use:'ts-loader',
            exclude: /node_modules/
        }]
    },
    resolve:{
        extensions:[".tsx", ".ts", ".js"]
    },
    output: {
        path: path.resolve('./wwwroot/scripts'),
        filename: 'app.min-typescript.js'
    }
};

export default config;