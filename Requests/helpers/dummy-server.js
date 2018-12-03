const express = require('express');
const app = express();

app.all('/*', function(req, res, next) {
    console.log('Intercepting requests ...');
    res.status(200).send('got it');
    next();
  });

  app.listen(3000, function () {
    console.log('Listening...');
  });