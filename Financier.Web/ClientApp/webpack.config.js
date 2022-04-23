const path = require("path");

const DEVELOPMENT = "development";
const PRODUCTION = "production";

module.exports = (env) => {
  var common = {
    output: {
      filename: "main.js",
      path: path.resolve(__dirname, "dist"),
    },
    resolve: {
      extensions: [".ts", ".tsx", ".js"]
    },
    module: {
      rules: [
        {
          test: /\.m?jsx?$/,
          exclude: /(node_modules|bower_components)/,
          use: {
            loader: 'babel-loader',
            options: {
              presets: ['@babel/preset-env', "@babel/preset-react"]
            }
          }
        },
        {
          test: [/\.css$/i, /\.scss$/i],
          use: [
            "style-loader",
            "css-loader",
            {
              loader: "sass-loader",
              options: {
                implementation: require("sass")
              }
            }
          ],
        },
        {
          test: [ /\.ts$/i, /\.tsx$/i ],
          use: ["ts-loader"],
        },
        {
          test: /\.(woff|woff2|eot|ttf|otf)$/i,
          type: 'asset/resource',
        },
      ],
    },
  };

  var configurations = {};
  configurations[DEVELOPMENT] = {
    name: DEVELOPMENT,
    mode: DEVELOPMENT,
    devtool: "inline-source-map",
    watch: false,
    entry: [
      `./src/index.tsx`,
    ],
  };
  configurations[PRODUCTION] = {
    name: PRODUCTION,
    mode: PRODUCTION,
    watch: false,
    entry: [
      `./src/index.${PRODUCTION}.tsx`,
    ],
  };

  return { ...common, ...configurations[env.mode] };
};
