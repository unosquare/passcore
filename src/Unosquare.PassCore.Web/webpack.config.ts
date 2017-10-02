import * as webpack from 'webpack';
import * as path from 'path';

const config: webpack.Configuration  = {
    entry: {
        polyfills: './wwwroot/polyfills.ts',
        app: './wwwroot/main.ts'
    },
    resolve: {
        extensions: ['.ts', '.js']
    },
    devtool: 'source-map',
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: 'ts-loader',
            }
        ]
    },

    output: {
        path: path.resolve('./wwwroot/scripts/tsscripts'),
        filename: '[name].typescript.js',
        sourceMapFilename: '[name].map',
    }
};

export default config;