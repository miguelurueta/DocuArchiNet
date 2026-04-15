import { Node, mergeAttributes } from "@tiptap/core";
import type { NodeType } from "@tiptap/pm/model";
import type { ResolvedPos } from "@tiptap/pm/model";

declare module "@tiptap/core" {
  interface Commands<ReturnType> {
    pageBreak: {
      insertPageBreak: () => ReturnType;
    };
  }
}

function resolveInsertionContext($from: ResolvedPos, nodeType: NodeType) {
  for (let depth = $from.depth; depth >= 0; depth -= 1) {
    const parent = $from.node(depth);
    const index = $from.index(depth);

    if (parent.canReplaceWith(index, index, nodeType)) {
      return {
        parent,
        index,
      };
    }
  }

  return null;
}

function hasAdjacentPageBreak(context: ReturnType<typeof resolveInsertionContext>, nodeName: string) {
  if (!context) {
    return false;
  }

  const previousNode = context.index > 0 ? context.parent.child(context.index - 1) : null;
  const nextNode =
    context.index < context.parent.childCount ? context.parent.child(context.index) : null;

  return previousNode?.type.name === nodeName || nextNode?.type.name === nodeName;
}

export const PageBreak = Node.create({
  name: "pageBreak",
  group: "block",
  atom: true,
  selectable: true,
  isolating: true,

  parseHTML() {
    return [
      {
        tag: 'div[data-page-break="true"]',
      },
    ];
  },

  renderHTML({ HTMLAttributes }) {
    return [
      "div",
      mergeAttributes(HTMLAttributes, {
        "data-page-break": "true",
      }),
    ];
  },

  addCommands() {
    return {
      insertPageBreak:
        () =>
        ({ state, commands }) => {
          const nodeType = state.schema.nodes[this.name];
          if (!nodeType) {
            return false;
          }

          const context = resolveInsertionContext(state.selection.$from, nodeType);
          if (!context || hasAdjacentPageBreak(context, this.name)) {
            return false;
          }

          return commands.insertContent({
            type: this.name,
          });
        },
    };
  },
});
