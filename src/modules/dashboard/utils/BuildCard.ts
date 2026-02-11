import type { MenuNode } from "../types/menu";

export default function BuildCard(tree: MenuNode[] = []): MenuNode[] {
  const result: MenuNode[] = [];

  const traverse = (nodes: unknown) => {
    if (!Array.isArray(nodes)) return;

    nodes.forEach((node: MenuNode) => {
      // ✅ Accesos directos (robusto)
      if (Number(node.AcesoDirecto) === 1) {
        result.push(node);
      }

      // 🔁 Recursión segura
      if (Array.isArray(node.children) && node.children.length > 0) {
        traverse(node.children);
      }
    });
  };

  traverse(tree);
  return result;
}
