/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.height = '300px';
    config.language = 'zh';
    config.extraPlugins = 'youtube,abbr';
    config.allowedContent = true;

    config.toolbar = 'Basic';
    config.toolbar = 'Full';

    config.toolbar_Full = [
		['Source', 'Undo', 'Redo', 'RemoveFormat', '-'],
		['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Print'],
		['Bold', 'Italic', 'Underline', 'Strike'],
		['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
		['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
		['Link', 'Unlink', 'Anchor'],
		['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe', 'youtube'],
		['Styles', 'Format', 'Font', 'FontSize', 'abbr'],
		['TextColor', 'BGColor'],
		['Maximize', 'ShowBlocks']
    ];

    config.font_names = 'Arial/Arial, Helvetica, sans-serif;Comic Sans MS/Comic Sans MS, cursive;Courier New/Courier New, Courier, monospace;Georgia/Georgia, serif;Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;Tahoma/Tahoma, Geneva, sans-serif;Times New Roman/Times New Roman, Times, serif;Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;Verdana/Verdana, Geneva, sans-serif;新細明體;標楷體;微軟正黑體';

    config.bodyClass = 'fr';

    config.filebrowserBrowseUrl = '../../../Areas/newweb/Content/ck/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '../../../Areas/newweb/Content/ck/ckfinder/ckfinder.html?type=Images';
    config.filebrowserFlashBrowseUrl = '../../../Areas/newweb/Content/ck/ckfinder/ckfinder.html?type=Flash';
    config.filebrowserUploadUrl = '../../../Areas/newweb/Content/ck/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '../../../Areas/newweb/Content/ck/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = '../../../Areas/newweb/Content/ck/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';

};
