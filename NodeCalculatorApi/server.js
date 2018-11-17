const express = require('express');
const cors = require('cors');
const app = express();
app.use(cors());
const jwt = require('express-jwt');
const jwksRsa = require('jwks-rsa');
const jwtAuthz = require('express-jwt-authz');
const jsonwebtoken = require('jsonwebtoken');
const { check, validationResult } = require('express-validator/check');

const checkJwt = jwt({
  secret: jwksRsa.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
    jwksUri: `https://apisummit.eu.auth0.com/.well-known/jwks.json`
  }),

  audience: 'calculator-api',
  issuer: `https://apisummit.eu.auth0.com/`,
  algorithms: ['RS256']
});

const doubleScope = jwtAuthz(['calc:double']);
const squareScope = jwtAuthz(['calc:square']);

app.get('/api/double/:number', checkJwt, doubleScope, [check('number').isInt()], function (req, res) {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(422).json({ errors: errors.array() });
  }

  var number = parseInt(req.params.number, 10);
  var double = number * 2;
  res.json({ result: double });
});

app.get('/api/square/:number', checkJwt, squareScope, function (req, res) {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(422).json({ errors: errors.array() });
  }

  var number = parseInt(req.params.number, 10);
  var square = number * number;
  res.json({ result: square });
});

app.get('/api/tokeninfo', checkJwt, function (req, res) {
  var decoded = jsonwebtoken.decode(req.headers.authorization.split(' ')[1]);
  res.status(200).send(decoded);
});

app.listen(5001, function () {
  console.log('Listening...');
});