// Rich Text Editor JavaScript functionality
let editor = null;
let dotNetRef = null;

window.initRichTextEditor = (editorElement, dotNetReference, initialContent) => {
    editor = editorElement;
    dotNetRef = dotNetReference;
    
    if (initialContent) {
        editor.innerHTML = initialContent;
    }
    
    editor.addEventListener('input', () => {
        dotNetRef.invokeMethodAsync('OnContentChanged', editor.innerHTML);
    });
};

window.executeEditorCommand = (command, value = null) => {
    document.execCommand(command, false, value);
};

window.insertHtmlAtCursor = (html) => {
    const selection = window.getSelection();
    if (selection.getRangeAt && selection.rangeCount) {
        const range = selection.getRangeAt(0);
        range.deleteContents();
        
        const div = document.createElement('div');
        div.innerHTML = html;
        const frag = document.createDocumentFragment();
        let node;
        
        while (node = div.firstChild) {
            frag.appendChild(node);
        }
        
        range.insertNode(frag);
    }
};

window.showImagePicker = async () => {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    
    return new Promise((resolve) => {
        input.onchange = async (e) => {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => resolve(e.target.result);
                reader.readAsDataURL(file);
            } else {
                resolve(null);
            }
        };
        input.click();
    });
};

window.getEditorContent = () => {
    return editor.innerHTML;
};

window.setEditorContent = (content) => {
    editor.innerHTML = content;
};