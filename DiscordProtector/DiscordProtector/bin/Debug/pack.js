var asar=require('asar')
var path=process.argv[2].replace(/\\/g,'/');
var cpath=process.argv[3].replace(/\\/g,'/');
asar.createPackage(`${path}`,`${cpath}`);