import { mergeAttributes } from "@tiptap/core";
import { Image } from "@tiptap/extension-image";
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

export const ResizableImage = Image.extend({
  addAttributes() {
    return {
      ...this.parent?.(),
      align: {
        default: "left",
        parseHTML: (element) => element.getAttribute("data-align") ?? "left",
        renderHTML: (attributes) => ({
          "data-align": attributes.align ?? "left",
        }),
      },
      width: {
        default: null,
        parseHTML: (element) =>
          element.getAttribute("data-width") ??
          element.getAttribute("width") ??
          extractWidth(element.getAttribute("style")),
        renderHTML: (attributes) => {
          if (!attributes.width) {
            return {};
          }

          return {
            "data-width": attributes.width,
            width: attributes.width,
            style: buildImageStyle(attributes.width, null),
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
        ({ editor, commands, state }) => {
          if (!editor.isActive("image") && !hasActiveImageSelection(state)) {
            return false;
          }

          return commands.updateAttributes("image", {
            align,
          });
        },
    };
  },

  renderHTML({ HTMLAttributes }) {
    const { width, style, ...restAttributes } = HTMLAttributes;

    return [
      "img",
      mergeAttributes(restAttributes, {
        ...(width ? { "data-width": width } : {}),
        style: buildImageStyle(width, style),
      }),
    ];
  },
});
