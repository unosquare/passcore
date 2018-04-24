// const { AngularCompilerPlugin } = require('@ngtools/webpack')
// const { AotPlugin } = require('@ngtools/webpack')
const { CommonsChunkPlugin } = require('webpack').optimize
const { SourceMapDevToolPlugin } = require('webpack')
const CircularDependencyPlugin = require('circular-dependency-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const path = require('path')
const WebpackCleanupPlugin = require('webpack-cleanup-plugin')
const webpack = require('webpack')

module.exports = {
  devtool: 'source-map',
  resolve: {
    extensions: ['.ts', '.js']
  },
  entry: {
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
      // { test: /\.ts$/, loader: '@ngtools/webpack' },
      {
        test: /\.js$/,
        exclude: /node_modules$/,
        loader: 'babel-loader'
      },
      {
        test: /\.ts$/,
        exclude: /node_modules$/,
        loaders: ['awesome-typescript-loader?useBabel=true', 'angular2-template-loader']
      },
      {
        test: /\.html|\.css$/,
        loader: 'raw-loader',
        exclude: [/node_modules$/, /\.async\.(html|css)$/]
      },
      {
        test: /\.async\.(html|css)$/,
        loaders: ['file?name=[name].[hash].[ext]', 'extract']
      }
    ]
  },
  plugins: [
    new WebpackCleanupPlugin(),
    new CircularDependencyPlugin({
      exclude: /node_modules$/, // exclude detection of files based on a RegExp
      failOnError: true, // add errors to webpack instead of warnings
      cwd: process.cwd() // set the current working directory for displaying module paths
    }),
    new CommonsChunkPlugin({
      name: ['main', 'vendor']
    }),
    new HtmlWebpackPlugin({
      template: './ClientApp/index.html'
    }),
    new SourceMapDevToolPlugin({
      'filename': '[file].map[query]',
      'moduleFilenameTemplate': '[resource-path]',
      'fallbackModuleFilenameTemplate': '[resource-path]?[hash]',
      'sourceRoot': 'webpack:///'
    }),
    /* new AotPlugin({
      'mainPath': 'main.ts',
      'replaceExport': false,
      'tsConfigPath': 'ClientApp\\tsconfig.app.json',
      'skipCodeGeneration': true
    }), */
    /* new AngularCompilerPlugin({
      'tsConfigPath': 'ClientApp\\tsconfig.app.json'
    }), */
    new webpack.ContextReplacementPlugin(/@angular(\\|\/)core(\\|\/)esm5/, path.join(__dirname, './client')),
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
