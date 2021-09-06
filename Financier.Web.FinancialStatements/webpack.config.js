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
      ],
    },
  };

  var configurations = {};
  configurations[DEVELOPMENT] = {
    name: DEVELOPMENT,
    mode: DEVELOPMENT,
    devtool: "inline-source-map",
    watch: true,
    entry: [
      `./src/index.${DEVELOPMENT}.tsx`,
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
