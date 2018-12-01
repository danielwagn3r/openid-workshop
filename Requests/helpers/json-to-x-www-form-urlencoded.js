var querystring = require('querystring')

var inputJson = {
    "scope":"user:read user:write"
}



console.log(querystring.stringify(inputJson))