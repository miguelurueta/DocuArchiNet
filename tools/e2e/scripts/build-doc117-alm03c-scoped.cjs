'use strict';
const {main}=require('./build-doc117-alm03-scoped.cjs'),{validateScope}=require('../tests/support/doc117-alm03c-scope.cjs'),{generate}=require('./support/doc117-alm03c-candidate.cjs');
if(require.main===module)try{main(process.argv.slice(2),{validateScope,generate});}catch{console.error('DOC117_ALM03C_BUILD_STOPPED');process.exitCode=2;}
module.exports={main:args=>main(args,{validateScope,generate})};
