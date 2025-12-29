const path = require('path');

module.exports = {
    entry: './ClientApp/index.js',
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),
        filename: 'hcaptcha-bundle.js',
        library: 'HCaptchaBundle',
        libraryTarget: 'umd',
    },
    module: {
        rules: [
            {
                test: /\.jsx?$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react'],
                    },
                },
            },
        ],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
    },
    externals: {
        react: 'React',
        'react-dom': 'ReactDOM',
    },
};
