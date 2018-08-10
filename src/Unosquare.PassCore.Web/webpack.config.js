﻿const { CommonsChunkPlugin } = require('webpack').optimize;
const { SourceMapDevToolPlugin } = require('webpack');

const CopyWebpackPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const WebpackCleanupPlugin = require('webpack-cleanup-plugin');

const webpack = require('webpack');
const path = require('path');

module.exports = {
  devServer: {
    historyApiFallback: true
  },
  resolve: {
    extensions: ['.ts', '.js']
  },
  entry: {
    'main': [ 'whatwg-fetch', './ClientApp/main.ts'],
    'vendor': './ClientApp/vendor.ts',
    'polyfills': './ClientApp/polyfills.ts'
  },
  output: {
    path: path.join(process.cwd(), './wwwroot'),
    filename: '[name].bundle.js',
    chunkFilename: '[id].chunk.js'
  },
  module: {
    rules: [
      { test: /\.html$/, loader: 'raw-loader' },
      { test: /\.css$/, loader: 'raw-loader' },
      { test: /\.ts$/, loaders: ['awesome-typescript-loader?useBabel=false', 'angular2-template-loader'] }
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
    new webpack.ContextReplacementPlugin(
      // The (\\|\/) piece accounts for path separators in *nix and Windows
      /angular(\\|\/)core(\\|\/)@angular/,
      './ClientApp', // location of your src
      {} // a map of your routes
    ),
    new webpack.ContextReplacementPlugin(
      // The (\\|\/) piece accounts for path separators in *nix and Windows
      /@angular(\\|\/)core(\\|\/)esm5/,
      './ClientApp', // location of your src
      {} // a map of your routes
      ),
    new SourceMapDevToolPlugin({
        "filename": '[file].map[query]',
        "moduleFilenameTemplate": '[resource-path]',
        "fallbackModuleFilenameTemplate": '[resource-path]?[hash]',
        "sourceRoot": 'webpack:///'
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
};
