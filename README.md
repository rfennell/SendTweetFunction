# Introduction 
A function to wrapper the sending of Tweets, handling the OAuth.

Useful if you wish to send Tweets from an Azure Logic App

# Usage
## Local
To run locally

```ps
$env:oauth_consumer_key ="xxx"
$env:oauth_consumer_secret = "xxx"
$env:oauth_token = "xxx-xxxx"
$env:oauth_token_secret = "xxxx"

func start
```

## Publish
To publish

```ps
az login
# in project root folder
# Assumes the above environment variables are set in the Azure Function 
func azure functionapp publish name_of_FuncName
```