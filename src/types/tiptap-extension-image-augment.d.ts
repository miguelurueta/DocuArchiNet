import "@tiptap/extension-image";

declare module "@tiptap/extension-image" {
  interface SetImageOptions {
    /**
     * App-level identity. Not used by tiptap itself, but carried as node attrs
     * by our custom image extension/node view.
     */
    imageId?: string;
    localImageId?: string;
  }
}

