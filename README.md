## Kudo
Kudo is a command-line tool to run commands on a cloud server using the Kudu SCM environment.

## Motivation
This tool was developed to assist with the automation of command-line commands on a remote Kudu server.

## Developed With
Developed in Visual Studio 15.8 using C# 7.0 and the Microsoft .NET Framework 4.6.2

## Features
* Uses a default per-user configuration file in JSON. (Defaults to %HOME%/.kudo/config.json)
* Can specify all parameters on command line.
* Writes remote command output to STDOUT or STDERR as if the command was executed locally.
* Returns the same Exit Code as if the command was executed locally.
* Not providing a username/password in the configuration or on the command line will prompt for them.

## Command Line Arguments
**-cfg**: The configuration file to load. Defaults to %HOME%/.kudo/config.json\
**-site**: The name of the site to use from the config file.\
**-baseUri**: The uri of the Kudu endpoint to run the command against.\
**-user**: The username to authenticate with.\
**-pass**: The password to authenticate with.\
**-cmd**: The command to run on the Kudu server.\
**-dir**: The directory on the Kudu server to run the command in.

## Configuration File
Sample Json Config:
```json
{
	"MyCoolSite" : {
		"baseUri": "https://mycoolsite.scm.azurewebsites.net/",
		"username": "$mycoolsite",
		"password": null,
		"command": null,
		"defaultdir": "D:\\home\\site\\wwwroot"
	},
	"MyOtherSite" : {
		"baseUri": "https://myothersite.scm.azurewebsites.net/",
		"username": "$myothersite",
		"password": "Correct Horse Battery Staple",
		"command": null,
		"defaultdir": "D:\\home\\site\\wwwroot"
	}
}
```

## Contribute
Pull requests are welcome! Feel free to fork or enhance!

## License
Copyright 2018 Web Matrix Pty Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.