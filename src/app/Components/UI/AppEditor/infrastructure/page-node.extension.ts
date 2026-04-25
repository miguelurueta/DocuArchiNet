import { Node, mergeAttributes } from "@tiptap/core";

export const PageNode = Node.create({
  name: "page",
  group: "block",
  content: "block+",
  defining: true,
  isolating: true,

  parseHTML() {
    return [
      {
        tag: 'div[data-app-editor-page="true"]',
      },
    ];
  },

  renderHTML({ HTMLAttributes }) {
    return [
      "div",
      mergeAttributes(HTMLAttributes, {
        "data-app-editor-page": "true",
      }),
      0,
    ];
  },
});
