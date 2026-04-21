import { mergeAttributes } from "@tiptap/core";
import type { CommandProps } from "@tiptap/core";
import type { Editor } from "@tiptap/core";
import { Image } from "@tiptap/extension-image";
import type { Node as ProseMirrorNode } from "@tiptap/pm/model";
import { NodeSelection } from "@tiptap/pm/state";
import type { EditorState } from "@tiptap/pm/state";

type ImageAlign = "left" | "center" | "right";

declare module "@tiptap/core" {
  interface Commands<ReturnType> {
    resizableImage: {
      setImageAlign: (align: ImageAlign) => ReturnType;
    };
  }
}

const WIDTH_STYLE_PATTERN = /width:\s*([^;]+)/i;

function extractWidth(style?: string | null) {
  if (!style) {
    return null;
  }

  const match = style.match(WIDTH_STYLE_PATTERN);
  return match?.[1]?.trim() ?? null;
}

function buildImageStyle(
  width?: string | null,
  style?: string | null,
) {
  const styleTokens = (style ?? "")
    .split(";")
    .map((token) => token.trim())
    .filter(Boolean)
    .filter((token) => !/^width:/i.test(token))
    .filter((token) => !/^max-width:/i.test(token))
    .filter((token) => !/^height:/i.test(token));

  styleTokens.push("display: block");
  if (width) {
    styleTokens.push(`width: ${width}`);
  }
  styleTokens.push("max-width: 100%");
  styleTokens.push("height: auto");

  return styleTokens.join("; ");
}

function hasActiveImageSelection(state: EditorState) {
  const selection = state.selection as
    | {
        node?: { type?: { name?: string } } | null;
        $anchor?: { parent?: { type?: { name?: string } } };
      }
    | undefined;

  if (selection?.node?.type?.name === "image") {
    return true;
  }

  return selection?.$anchor?.parent?.type?.name === "image";
}

function applyNodeViewImageAttributes(
  wrapper: HTMLDivElement,
  image: HTMLImageElement,
  node: ProseMirrorNode,
) {
  const align = typeof node.attrs.align === "string" ? node.attrs.align : "left";
  const width = typeof node.attrs.width === "string" ? node.attrs.width : null;
  const imageId = typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
  const localImageId =
    typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
  const source = typeof node.attrs.source === "string" ? node.attrs.source : null;
  const src = typeof node.attrs.src === "string" ? node.attrs.src : "";
  const alt = typeof node.attrs.alt === "string" ? node.attrs.alt : "";
  const title = typeof node.attrs.title === "string" ? node.attrs.title : "";

  wrapper.setAttribute("data-align", align);
  wrapper.setAttribute("data-app-editor-image-node", "true");

  image.setAttribute("src", src);
  image.setAttribute("alt", alt);
  image.style.cssText = buildImageStyle(width, null);

  if (title) {
    image.setAttribute("title", title);
  } else {
    image.removeAttribute("title");
  }

  if (align) {
    image.setAttribute("data-align", align);
  } else {
    image.removeAttribute("data-align");
  }

  if (width) {
    image.setAttribute("data-width", width);
    image.setAttribute("width", width);
  } else {
    image.removeAttribute("data-width");
    image.removeAttribute("width");
  }

  if (imageId) {
    wrapper.setAttribute("data-image-id", imageId);
    image.setAttribute("data-image-id", imageId);
  } else {
    wrapper.removeAttribute("data-image-id");
    image.removeAttribute("data-image-id");
  }

  if (localImageId) {
    wrapper.setAttribute("data-local-image-id", localImageId);
    image.setAttribute("data-local-image-id", localImageId);
  } else {
    wrapper.removeAttribute("data-local-image-id");
    image.removeAttribute("data-local-image-id");
  }

  if (source) {
    wrapper.setAttribute("data-source", source);
    image.setAttribute("data-source", source);
  } else {
    wrapper.removeAttribute("data-source");
    image.removeAttribute("data-source");
  }

  if (src) {
    wrapper.setAttribute("data-src", src);
  } else {
    wrapper.removeAttribute("data-src");
  }
}

function applyNodeViewSelectionState(
  wrapper: HTMLDivElement,
  image: HTMLImageElement,
  isActive: boolean,
) {
  if (isActive) {
    wrapper.classList.add("ProseMirror-selectednode");
    wrapper.setAttribute("data-selected", "true");
    wrapper.setAttribute("data-app-editor-image-active", "true");
    wrapper.setAttribute("data-app-editor-image-persistent", "true");
    image.setAttribute("data-app-editor-image-active", "true");
    image.setAttribute("data-app-editor-image-persistent", "true");
    return;
  }

  wrapper.classList.remove("ProseMirror-selectednode");
  wrapper.removeAttribute("data-selected");
  wrapper.removeAttribute("data-app-editor-image-active");
  wrapper.removeAttribute("data-app-editor-image-persistent");
  image.removeAttribute("data-app-editor-image-active");
  image.removeAttribute("data-app-editor-image-persistent");
}

function matchesActiveImageIdentity(
  node: ProseMirrorNode,
  identity:
    | {
        imageId?: string | null;
        localImageId?: string | null;
        src?: string | null;
      }
    | null
    | undefined,
) {
  if (!identity) {
    return false;
  }

  const nodeImageId = typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
  const nodeLocalImageId =
    typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
  const nodeSrc = typeof node.attrs.src === "string" ? node.attrs.src : null;

  return Boolean(
    (identity.imageId && nodeImageId === identity.imageId) ||
      (identity.localImageId && nodeLocalImageId === identity.localImageId) ||
      (identity.src && nodeSrc === identity.src),
  );
}

function clearActiveImageIndicators(root: HTMLElement) {
  Array.from(root.querySelectorAll("[data-app-editor-image-node='true']")).forEach((node) => {
    if (!(node instanceof HTMLElement)) {
      return;
    }

    node.classList.remove("ProseMirror-selectednode");
    node.removeAttribute("data-selected");
    node.removeAttribute("data-app-editor-image-active");
    node.removeAttribute("data-app-editor-image-persistent");

    const image = node.querySelector("img");
    if (image instanceof HTMLImageElement) {
      image.removeAttribute("data-app-editor-image-active");
      image.removeAttribute("data-app-editor-image-persistent");
    }
  });
}

export const ResizableImage = Image.extend({
  addAttributes() {
    return {
      ...this.parent?.(),
      align: {
        default: "left",
        parseHTML: (element: HTMLElement) => element.getAttribute("data-align") ?? "left",
        renderHTML: (attributes: Record<string, unknown>) => ({
          "data-align":
            typeof attributes.align === "string" ? attributes.align : "left",
        }),
      },
      localImageId: {
        default: null,
        parseHTML: (element: HTMLElement) => element.getAttribute("data-local-image-id"),
        renderHTML: (attributes: Record<string, unknown>) =>
          typeof attributes.localImageId === "string" && attributes.localImageId
            ? {
                "data-local-image-id": attributes.localImageId,
              }
            : {},
      },
      imageId: {
        default: null,
        parseHTML: (element: HTMLElement) => element.getAttribute("data-image-id"),
        renderHTML: (attributes: Record<string, unknown>) =>
          typeof attributes.imageId === "string" && attributes.imageId
            ? {
                "data-image-id": attributes.imageId,
              }
            : {},
      },
      source: {
        default: null,
        parseHTML: (element: HTMLElement) => element.getAttribute("data-source"),
        renderHTML: (attributes: Record<string, unknown>) =>
          typeof attributes.source === "string" && attributes.source
            ? {
                "data-source": attributes.source,
              }
            : {},
      },
      width: {
        default: null,
        parseHTML: (element: HTMLElement) =>
          element.getAttribute("data-width") ??
          element.getAttribute("width") ??
          extractWidth(element.getAttribute("style")),
        renderHTML: (attributes: Record<string, unknown>) => {
          const width = typeof attributes.width === "string" ? attributes.width : null;
          if (!width) {
            return {};
          }

          return {
            "data-width": width,
            width,
            style: buildImageStyle(width, null),
          };
        },
      },
    };
  },

  addCommands() {
    return {
      ...this.parent?.(),
      setImageAlign:
        (align: ImageAlign) =>
        ({ editor, commands, state }: CommandProps) => {
          if (!editor.isActive("image") && !hasActiveImageSelection(state)) {
            return false;
          }

          return commands.updateAttributes("image", {
            align,
          });
      },
    };
  },

  addNodeView() {
    return ({
      node,
      editor,
      getPos,
    }: {
      node: ProseMirrorNode;
      editor: Editor;
      getPos?: () => number | undefined;
    }) => {
      let currentNode = node;
      const wrapper = document.createElement("div");
      wrapper.className = "app-editor-image-node";
      wrapper.contentEditable = "false";

      const image = document.createElement("img");
      image.draggable = false;
      wrapper.appendChild(image);

      const resolvePosition = () =>
        typeof getPos === "function" ? getPos() : null;

      const syncSelectionStateFromEditor = () => {
        const position = resolvePosition();
        const selection = editor.state.selection;
        const activeImageIdentity = (
          editor as typeof editor & {
            __appEditorLastImageIdentity?: {
              imageId?: string | null;
              localImageId?: string | null;
              src?: string | null;
            } | null;
          }
        ).__appEditorLastImageIdentity;
        const isSelected =
          typeof position === "number" &&
          selection instanceof NodeSelection &&
          selection.node.type.name === "image" &&
          selection.from === position;
        const isPersistedActive = matchesActiveImageIdentity(currentNode, activeImageIdentity);

        applyNodeViewSelectionState(wrapper, image, isSelected || isPersistedActive);
      };

      const selectCurrentImage = () => {
        const position = resolvePosition();
        if (typeof position !== "number") {
          return false;
        }

        const selection = editor.state.selection;
        const alreadySelected =
          selection instanceof NodeSelection &&
          selection.node.type.name === "image" &&
          selection.from === position;

        if (alreadySelected) {
          return true;
        }

        const transaction = editor.state.tr.setSelection(
          NodeSelection.create(editor.state.doc, position),
        );
        editor.view.dispatch(transaction);
        clearActiveImageIndicators(editor.view.dom);
        (editor as Editor & {
          __appEditorLastImagePos?: number | null;
          __appEditorLastImageIdentity?: {
            imageId?: string | null;
            localImageId?: string | null;
            src?: string | null;
          } | null;
        }).__appEditorLastImagePos = position;
        (editor as Editor & {
          __appEditorLastImageIdentity?: {
            imageId?: string | null;
            localImageId?: string | null;
            src?: string | null;
          } | null;
        }).__appEditorLastImageIdentity = {
          imageId: typeof node.attrs.imageId === "string" ? node.attrs.imageId : null,
          localImageId:
            typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null,
          src: typeof node.attrs.src === "string" ? node.attrs.src : null,
        };
        syncSelectionStateFromEditor();
        editor.view.focus();
        return true;
      };

      const handlePointerSelect = (event: MouseEvent) => {
        event.preventDefault();
        event.stopPropagation();
        selectCurrentImage();
      };

      wrapper.addEventListener("mousedown", handlePointerSelect);
      wrapper.addEventListener("click", handlePointerSelect);
      applyNodeViewImageAttributes(wrapper, image, node);
      syncSelectionStateFromEditor();

      return {
        dom: wrapper,
        update: (updatedNode: ProseMirrorNode) => {
          if (updatedNode.type.name !== node.type.name) {
            return false;
          }

          currentNode = updatedNode;
          applyNodeViewImageAttributes(wrapper, image, updatedNode);
          syncSelectionStateFromEditor();
          return true;
        },
        selectNode: () => {
          applyNodeViewSelectionState(wrapper, image, true);
        },
        deselectNode: () => {
          syncSelectionStateFromEditor();
        },
        stopEvent: (event: Event) =>
          event.type === "mousedown" ||
          event.type === "click" ||
          event.type === "dragstart",
        ignoreMutation: () => true,
        destroy: () => {
          wrapper.removeEventListener("mousedown", handlePointerSelect);
          wrapper.removeEventListener("click", handlePointerSelect);
        },
      };
    };
  },

  renderHTML({ HTMLAttributes }: { HTMLAttributes: Record<string, unknown> }) {
    const { width, style, ...restAttributes } = HTMLAttributes as Record<
      string,
      string | number | boolean | null | undefined
    >;
    const widthValue =
      typeof width === "string"
        ? width
        : typeof width === "number"
          ? String(width)
          : null;
    const styleValue = typeof style === "string" ? style : null;

    return [
      "img",
      mergeAttributes(restAttributes, {
        ...(widthValue ? { "data-width": widthValue } : {}),
        style: buildImageStyle(widthValue, styleValue),
      }),
    ];
  },
});
