var asar=require('asar')
var path=process.argv[2].replace(/\\/g,'/');
var epath=process.argv[3].replace(/\\/g,'/');
asar.extractAll(`${path}`,`${epath}`);