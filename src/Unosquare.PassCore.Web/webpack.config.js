const { CommonsChunkPlugin } = require('webpack').optimize
const BrotliPlugin = require('brotli-webpack-plugin')
const CircularDependencyPlugin = require('circular-dependency-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin')
const CssoWebpackPlugin = require('csso-webpack-plugin').default
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const path = require('path')
const WebpackCleanupPlugin = require('webpack-cleanup-plugin')
const webpack = require('webpack')

module.exports = {
  devtool: 'source-map',
  devServer: {
    historyApiFallback: true
  },
  resolve: {
    extensions: ['.ts', '.js', '.css']
  },
  entry: {
    'main': './ClientApp/main.ts',
    'vendor': './ClientApp/vendor.ts',
    'polyfills': './ClientApp/polyfills.ts'
  },
  output: {
    path: path.join(process.cwd(), './wwwroot'),
    filename: '[name].bundle.js',
    chunkFilename: '[id].chunk.js',
    sourceMapFilename: '[file].map'
  },
  module: {
    rules: [
      {
        test: /\.json$/,
        loader: 'json-loader',
        exclude: [/node_modules$/]
      },
      {
        test: /\.css$/,
        loaders: ['to-string-loader'].concat(ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use: ['css-loader?sourceMap', 'postcss-loader']
        })),
        exclude: [/node_modules$/]
      },
      {
        test: /\.ts$/,
        loaders: ['awesome-typescript-loader?useBabel=false', 'angular2-template-loader'],
        exclude: [/node_modules$/]
      },
      {
        test: /\.(html)$/,
        loader: 'raw-loader',
        exclude: [/node_modules$/, /\.async\.(html|css)$/]
      },
      {
        test: /\.async\.(html)$/,
        loaders: ['file?name=[name].[hash].[ext]', 'extract'],
        exclude: [/node_modules$/]
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
      name: ['main', 'vendor', 'polyfills', 'styles']
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
    new ExtractTextPlugin('assets/styles/styles.css'),
    new CssoWebpackPlugin(),
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
    }),
    new BrotliPlugin({
      asset: '[path].br[query]',
      test: /\.(js|css|html|svg|map)$/,
      threshold: 10240,
      minRatio: 0.8
    })
  ]
}
