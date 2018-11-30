const crypto = require('crypto');
const base64url = require('base64url');


var code_verifier = base64url.encode(crypto.randomBytes(32));

console.log(`code verifier: ${code_verifier}`);

var hash = crypto.createHash('sha256').update(code_verifier).digest();
var code_challenge = base64url.encode(hash)

console.log(`code challenge: ${code_challenge}`);
