# Overview

Creator Console needs AWS credential to be set. It uses the profile named "content_use".
So make sure there is profile section shown as the following in the file of the path
$HOME/.aws/credentials

```ini
[default]
aws_access_key_id=
aws_secret_access_key=

[content_use]
aws_access_key_id=
aws_secret_access_key=
```

Check the following documents for more detail

[Authentication and access credentials](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-authentication.html)
[Configuration and credential file settings](https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-files.html)
