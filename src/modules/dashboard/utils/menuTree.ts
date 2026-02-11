// import type { MenuNode, RawMenuItem } from "../types/menu";

// /**
//  * Construye árbol del menú a partir de la lista plana.
//  */
// export const buildMenuTree = (items: RawMenuItem[]): MenuNode[] => {
//   const byId = new Map<number, MenuNode>();

//   items.forEach((item) => {
//     byId.set(item.IdMenuPrincipal, { ...item, children: [] });
//   });

//   const roots: MenuNode[] = [];

//   byId.forEach((node) => {
//     if (node.IdPadre === 0 || !byId.has(node.IdPadre)) {
//       roots.push(node);
//       return;
//     }

//     const parent = byId.get(node.IdPadre);
//     if (parent) {
//       parent.children.push(node);
//     }
//   });

//   const filterVisible = (node: MenuNode): MenuNode | null => {
//     const filteredChildren = node.children
//       .map(filterVisible)
//       .filter((child): child is MenuNode => Boolean(child));
//     const isVisible = node.VisibleNode === 1 || filteredChildren.length > 0;
//     if (!isVisible) {
//       return null;
//     }
//     return { ...node, children: filteredChildren };
//   };

//   const filteredRoots = roots
//     .map(filterVisible)
//     .filter((node): node is MenuNode => Boolean(node));

//   const sortNodes = (nodes: MenuNode[]) => {
//     nodes.sort((a, b) => a.Orden - b.Orden);
//     nodes.forEach((node) => sortNodes(node.children));
//   };

//   sortNodes(filteredRoots);

//   return filteredRoots;
// };

// /**
//  * Extrae elementos con acceso directo para las tarjetas del dashboard.
//  */
// export const getDirectAccessItems = (items: RawMenuItem[]) =>
//   items
//     .filter((item) => item.AcesoDirecto === 1 && item.VisibleNode === 1)
//     .sort((a, b) => a.Orden - b.Orden);
