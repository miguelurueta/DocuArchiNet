/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    //config.removeButtons = 'Underline,Subscript,Superscript';
    // Set the most common block elements.
    config.format_tags = 'p;h1;h2;h3;pre;';
    //config.format_p = { element: 'p', attributes: { 'class': 'normalPara' } };
    // Simplify the dialog windows.
    //config.removeDialogTabs = 'image:advanced;link:advanced';
    config.baseFloatZIndex = 1000002;
    config.allowedContent = true;
    config.height = 300;
    config.enterMode = CKEDITOR.ENTER_P; // <p></p> to <br />
    config.entities = false;
    config.basicEntities = false;
};
