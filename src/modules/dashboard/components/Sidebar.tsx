import { Layout, Menu, Spin, Tooltip, Badge } from "antd";
import { useState, useMemo, useRef, useEffect } from "react";
import type { MenuProps } from "antd";
import type { MenuNode } from "../types/menu";
import styles from "../style/side.module.css";
import { useNavigate, useLocation } from "react-router-dom";

const { Sider } = Layout;

interface SidebarProps {
  collapsed: boolean;
  onCollapse: (collapsed: boolean) => void;
  menuTree: MenuNode[] | [];
  metricMap: Map<number, number>;
  isLoading: boolean;
}

export default function Sidebar({
  collapsed,
  onCollapse,
  menuTree,
  metricMap,
  isLoading,
}: SidebarProps) {
  const [openKeys, setOpenKeys] = useState<string[]>([]);
  const siderRef = useRef<HTMLDivElement>(null);

  const navigate = useNavigate();
  const location = useLocation();

  /* ---------- MAPAS ---------- */
  const { keyParentMap, keyUrlMap } = useMemo(() => {
    const parentMap: Record<string, string | null> = {};
    const urlMap: Record<string, string> = {};

    const walk = (nodes: MenuNode[], parent: string | null) => {
      nodes.forEach((node) => {
        const key = String(node.IdMenuPrincipal);
        parentMap[key] = parent;

        if (node.UrlNode) urlMap[key] = node.UrlNode;
        if (node.children?.length) walk(node.children, key);
      });
    };

    walk(menuTree, null);
    return { keyParentMap: parentMap, keyUrlMap: urlMap };
  }, [menuTree]);

  /* ---------- SINCRONIZAR CON RUTA ---------- */
  useEffect(() => {
    if (collapsed) return;

    const selectedEntry = Object.entries(keyUrlMap).find(
      ([, url]) =>
        location.pathname === url ||
        location.pathname.startsWith(url + "/")
    );

    if (selectedEntry) {
      const selectedKey = selectedEntry[0];

      const parents: string[] = [];
      let parent = keyParentMap[selectedKey];

      while (parent) {
        parents.push(parent);
        parent = keyParentMap[parent];
      }

      setOpenKeys(parents);
    }
  }, [location.pathname, keyParentMap, keyUrlMap, collapsed]);

  /* ---------- ACCORDION ---------- */
  const onOpenChange = (keys: string[]) => {
    const latest = keys.find((k) => !openKeys.includes(k));
    if (!latest) return setOpenKeys(keys);

    const parent = keyParentMap[latest];

    if (!parent) {
      setOpenKeys([latest]);
    } else {
      setOpenKeys(keys);
    }
  };

  /* ---------- CLICK ---------- */
  const onMenuClick: MenuProps["onClick"] = ({ key }) => {
    const url = keyUrlMap[key as string];
    if (url) navigate(url);
  };

  /* ---------- ACTIVO ---------- */
  const selectedKeys = useMemo(() => {
    const entry = Object.entries(keyUrlMap).find(
      ([, url]) =>
        location.pathname === url ||
        location.pathname.startsWith(url + "/")
    );
    return entry ? [entry[0]] : [];
  }, [location.pathname, keyUrlMap]);

  /* ---------- ITEMS ---------- */
  const buildMenuItems = (nodes: MenuNode[]): MenuProps["items"] =>
    nodes
      .filter((n) => n.VisibleNode === 1)
      .sort((a, b) => a.Orden - b.Orden)
      .map((node) => {
        const key = String(node.IdMenuPrincipal);
        const pending = metricMap.get(node.IdMenuPrincipal) ?? 0;

        const expandedLabel = (
          <div className={styles.menuRow}>
            <span className={styles.menuText}>
              {node.NombreModulo}
            </span>

            {pending > 0 && (
              <Badge
                count={pending}
                size="small"
                overflowCount={99}
                className={styles.menuBadge}
              />
            )}
          </div>
        );

        const collapsedLabel = (
          <Tooltip
            title={node.ToltipNode || node.NombreModulo}
            placement="right"
            color="#505050"
          >
            <span>{node.NombreModulo}</span>
          </Tooltip>
        );

        return {
          key,
          icon: node.Icono ? <i className={node.Icono} /> : undefined,
          label: collapsed ? collapsedLabel : expandedLabel,
          children: node.children?.length
            ? buildMenuItems(node.children)
            : undefined,
        };
      });

  const menuItems = useMemo(
    () => buildMenuItems(menuTree),
    [menuTree, collapsed, metricMap]
  );

  return (
    <Sider
      ref={siderRef}
      collapsible
      collapsed={collapsed}
      onCollapse={onCollapse}
      trigger={null}
      width={300}
      collapsedWidth={70}
      theme="light"
      className={styles.sider}
    >
      <div className={styles.logo}>
        <img
          src={
            collapsed
              ? "/src/assets/docuArchiD.png"
              : "/src/assets/docuArchi.png"
          }
          alt="DocuArchi"
        />
      </div>

      {isLoading ? (
        <div className={styles.loading}>
          <Spin />
        </div>
      ) : (
        <Menu
          mode="inline"
          items={menuItems}
          inlineCollapsed={collapsed}
          getPopupContainer={() =>
            collapsed ? document.body : (siderRef.current ?? document.body)
          }
          selectedKeys={selectedKeys}
          openKeys={collapsed ? undefined : openKeys}
          onOpenChange={collapsed ? undefined : onOpenChange}
          onClick={onMenuClick}
          className={styles.menu}
        />
      )}
    </Sider>
  );
}
