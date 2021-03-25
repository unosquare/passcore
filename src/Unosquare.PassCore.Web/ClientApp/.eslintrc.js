module.exports = {
    parser: '@typescript-eslint/parser', // Specifies the ESLint parser
    extends: [
        'eslint-config-standard-react', // https://github.com/standard/eslint-config-standard-react
        'standard-with-typescript', // https://github.com/standard/eslint-config-standard-with-typescript
        'plugin:@typescript-eslint/recommended', // Uses the recommended rules from @typescript-eslint/eslint-plugin
        'plugin:@typescript-eslint/recommended-requiring-type-checking', // https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/eslint-plugin
        'prettier/@typescript-eslint', // Uses eslint-config-prettier to disable ESLint rules from @typescript-eslint/eslint-plugin that would conflict with prettier
        'plugin:prettier/recommended', // Enables eslint-plugin-prettier and eslint-config-prettier. This will display prettier errors as ESLint errors. Make sure this is always the last configuration in the extends array.
    ],
    parserOptions: {
        ecmaVersion: 2018, // Allows for the parsing of modern ECMAScript features
        sourceType: 'module', // Allows for the use of imports
        ecmaFeatures: {
            jsx: true, // Allows for the parsing of JSX
        },
        project: './tsconfig.json'
    },
    rules: {
        // Place to specify ESLint rules. Can be used to overwrite rules specified from the extended configs
        //'@typescript-eslint/explicit-function-return-type': 'off',
        //'@typescript-eslint/interface-name-prefix': 'off',
        //'@typescript-eslint/prop-types': 'off',
        //'@typescript-eslint/ban-ts-ignore': 'off',
        //'@typescript-eslint/strictNullChecks': 1,
    },
    settings: {
        react: {
            version: 'detect', // Tells eslint-plugin-react to automatically detect the version of React to use
        },
    },
};
