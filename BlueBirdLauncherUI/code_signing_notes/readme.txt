To digitally sign for release, you need a certificate.pfx file and the password to that file

Then tun the Developer Command Prompt for VS2019 console

Execute the following command:
signtool.exe sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /f "{path_to_bluebirdrp_launcher.pfx}" /as /p "{password_for_bluebirdrp_launcher.pfx}" "{path_to_BlueBirdLauncherUI.exe}"

Output should say:
Done Adding Additional Store
Successfully signed: {path_to_BlueBirdLauncherUI.exe}