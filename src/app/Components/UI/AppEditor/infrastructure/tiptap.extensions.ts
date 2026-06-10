import type { AnyExtension } from "@tiptap/react";
import { Placeholder } from "@tiptap/extension-placeholder";
import { Underline } from "@tiptap/extension-underline";
import { Link } from "@tiptap/extension-link";
import { TaskList } from "@tiptap/extension-task-list";
import { TaskItem } from "@tiptap/extension-task-item";
import { TextAlign } from "@tiptap/extension-text-align";
import { StarterKit } from "@tiptap/starter-kit";
import { ResizableImage } from "./resizable-image.extension";
import { PageDocument } from "./page-document.extension";
import { PageNode } from "./page-node.extension";

type BuildAppEditorExtensionsOptions = {
  paginatedDocument?: boolean;
};

export function buildAppEditorExtensions(
  placeholder?: string,
  options?: BuildAppEditorExtensionsOptions,
): AnyExtension[] {
  const paginatedDocument = options?.paginatedDocument === true;

  return [
    StarterKit.configure({
      document: paginatedDocument ? false : undefined,
      heading: {
        levels: [1, 2, 3],
      },
      link: false,
      underline: false,
      undoRedo: false,
    }),
    ...(paginatedDocument ? [PageDocument, PageNode] : []),
    Placeholder.configure({
      placeholder: placeholder?.trim() || "Escribe aqui...",
    }),
    Underline,
    Link.configure({
      openOnClick: false,
      autolink: false,
      defaultProtocol: "https",
      HTMLAttributes: {
        rel: "noopener noreferrer",
        target: "_blank",
      },
    }),
    ResizableImage.configure({
      inline: false,
      allowBase64: true,
    }),
    TaskList,
    TaskItem.configure({
      nested: true,
    }),
    TextAlign.configure({
      types: ["heading", "paragraph"],
      alignments: ["left", "center", "right", "justify"],
      defaultAlignment: "left",
    }),
  ];
}
