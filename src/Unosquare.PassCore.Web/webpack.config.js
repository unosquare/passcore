const { CommonsChunkPlugin } = require('webpack').optimize;
const { SourceMapDevToolPlugin } = require('webpack');
const { AotPlugin } = require('@ngtools/webpack');

const CopyWebpackPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const WebpackCleanupPlugin = require('webpack-cleanup-plugin');

const path = require('path');

module.exports = {
    resolve: {
        extensions: ['.ts', '.js']
    },
    entry: {
        'polyfills': './ClientApp/polyfills.ts',
        'main': './ClientApp/main.ts',
        'vendor': './ClientApp/vendor.ts'
    },
    output: {
        path: path.join(process.cwd(), './wwwroot'),
        filename: '[name].bundle.js',
        chunkFilename: '[id].chunk.js'
    },
    module: {
        rules: [
            { test: /\.ts$/, loader: '@ngtools/webpack'  },
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.css$/, loader: 'raw-loader' }
        ]
    },
    plugins: [
        new WebpackCleanupPlugin(),
        new CommonsChunkPlugin({
            name: ['main', 'vendor', 'polyfills']
        }),
        new HtmlWebpackPlugin({
            template: './ClientApp/index.html'
        }),
        new SourceMapDevToolPlugin({
            "filename": '[file].map[query]',
            "moduleFilenameTemplate": '[resource-path]',
            "fallbackModuleFilenameTemplate": '[resource-path]?[hash]',
            "sourceRoot": 'webpack:///'
        }),
        new AotPlugin({
            'mainPath': 'main.ts',
            'replaceExport': false,
            'tsConfigPath': 'ClientApp\\tsconfig.app.json',
            'skipCodeGeneration': true
        }),
        new CopyWebpackPlugin([{
            'context': 'ClientApp',
            'to': '',
            'from': {
                'glob': 'manifest.json',
                'dot': true
            }
        }, {
            'context': 'ClientApp',
            'to': '',
            'from': {
                'glob': 'assets/**/*',
                'dot': true
            }
        },
        {
            'context': 'ClientApp',
            'to': '',
            'from': {
                'glob': 'favicon.ico',
                'dot': true
            }
        },
        {
            'context': 'ClientApp',
            'to': '',
            'from': {
                'glob': 'web.config',
                'dot': true
            }
        }
        ], {
            'ignore': [
                '.gitkeep'
            ],
            'debug': 'warning'
        })
    ]
}