// import type { MenuNode } from "../types/menu";

import type { MenuNode, RawMenuItem } from "../types/menu";

// export function buildMenuTreeLegacy(items: MenuNode[]): MenuNode[] {
//   const map = new Map<number, MenuNode>();
//   const roots: MenuNode[] = [];

//   // Registrar TODOS los nodos
//   items.forEach(item => {
//     map.set(item.IdMenuPrincipal, {
//       ...item,
//       children: []
//     });
//   });

//   // Relacionar padre → hijos
//   map.forEach(item => {
//     if (item.IdPadre === 0) {
//       roots.push(item);
//       return;
//     }

//     const parent = map.get(item.IdPadre);

//     if (parent) {
//       parent.children!.push(item);
//     } else {
//       // fallback legacy: no perder nodos
//       roots.push(item);
//     }
//   });

//   return roots;
// }


//===========================================================================================
//PRUEBA

// utils/buildMenuTree.ts
export const buildMenuTree = (items: RawMenuItem[]): MenuNode[] => {
  const byId = new Map<number, MenuNode>();

  items.forEach(item => {
    byId.set(item.IdMenuPrincipal, { ...item, children: [] });
  });

  const roots: MenuNode[] = [];

  byId.forEach(node => {
    if (node.IdPadre === 0 || !byId.has(node.IdPadre)) {
      roots.push(node);
    } else {
      byId.get(node.IdPadre)!.children.push(node);
    }
  });

  const filterVisible = (node: MenuNode): MenuNode | null => {
    const children = node.children
      .map(filterVisible)
      .filter(Boolean) as MenuNode[];

    if (node.VisibleNode !== 1 && children.length === 0) return null;

    return { ...node, children };
  };

  const filtered = roots
    .map(filterVisible)
    .filter(Boolean) as MenuNode[];

  const sort = (nodes: MenuNode[]) => {
    nodes.sort((a, b) => a.Orden - b.Orden);
    nodes.forEach(n => sort(n.children));
  };

  sort(filtered);
  return filtered;
};



