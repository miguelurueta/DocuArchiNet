'use strict';
const {createControls}=require('./doc117-alm03-controls.cjs');
const {readBinding,validateBinding}=require('../../scripts/support/doc117-alm03-canonical-description.cjs');
function createCanonicalControls(query,scope,profile,binding){return createControls(query,scope,profile,{descriptionBinding:validateBinding(binding)});}
module.exports={createCanonicalControls,readBinding};
