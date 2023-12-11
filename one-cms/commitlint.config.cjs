module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    // @see https://commitlint.js.org/#/reference-rules
    'type-enum': [
      2,
      'always',
      [
        'feat', // add new feature
        'fix', // fix bug
        'docs', // docs change
        'style', // style format
        'refactor', // refactor
        'perf', // performance
        'test', // test case
        'build', // build process
        'ci', // ci process
        'revert', // revert commit
        'chore', // chore
      ],
    ],
    'subject-case': [0],
    'type-case': [0],
    'type-empty': [0],
    'scope-empty': [0],
    'scope-case': [0],
    'subject-full-stop': [0, 'never'],
    'header-max-length': [2, 'always', 72],
  },
};
