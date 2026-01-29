import {
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Popover,
  Tooltip,
  Typography,
} from "@mui/material";
import { useMemo, useState, type MouseEvent } from "react";
import { NavLink } from "react-router";
import { useMenuItems } from "../hooks/useMenuItems";
import { buildMenuTree } from "../utils/menuTree";
import type { MenuNode } from "../types/menu";

interface SidebarProps {
  isOpen: boolean;
  isMobile: boolean;
  mobileOpen: boolean;
  onClose: () => void;
}

const drawerWidth = 260;
const collapsedWidth = 78;

/**
 * Sidebar dinámico basado en el árbol de menú proporcionado por la API.
 */
const Sidebar = ({ isOpen, isMobile, mobileOpen, onClose }: SidebarProps) => {
  const { data, isLoading } = useMenuItems();
  const menuTree = useMemo(() => buildMenuTree(data ?? []), [data]);
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [activeParent, setActiveParent] = useState<MenuNode | null>(null);

  const handleOpenMenu = (event: MouseEvent<HTMLElement>, node: MenuNode) => {
    setAnchorEl(event.currentTarget);
    setActiveParent(node);
  };

  const handleCloseMenu = () => {
    setAnchorEl(null);
    setActiveParent(null);
  };

  const renderMenuIcon = (icon?: string) => (
    <Box
      component="span"
      sx={{
        minWidth: 24,
        display: "inline-flex",
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      {icon ? <i className={icon} aria-hidden /> : <i className="fa-regular fa-circle" aria-hidden />}
    </Box>
  );

  const drawerContent = (
    <Box
      sx={{
        height: "100%",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Box
        sx={{
          py: 3,
          px: isOpen ? 3 : 2,
          display: "flex",
          alignItems: "center",
          gap: 1.5,
        }}
      >
        <Box
          sx={{
            width: 36,
            height: 36,
            borderRadius: 2,
            backgroundColor: "primary.main",
            color: "primary.contrastText",
            display: "grid",
            placeItems: "center",
            fontWeight: 700,
          }}
        >
          D
        </Box>
        {isOpen && (
          <Typography variant="subtitle1" fontWeight={700}>
            Docuarchi
          </Typography>
        )}
      </Box>

      <List sx={{ flex: 1, px: 1 }}>
        {isLoading && (
          <Typography variant="body2" color="text.secondary" sx={{ px: 2 }}>
            Cargando menú...
          </Typography>
        )}
        {menuTree.map((node) => {
          const hasChildren = node.children.length > 0;
          const listItem = (
            <ListItemButton
              key={node.ValueNode}
              onMouseEnter={(event) => hasChildren && !isMobile && handleOpenMenu(event, node)}
              onFocus={(event) => hasChildren && handleOpenMenu(event, node)}
              onClick={(event) => hasChildren && handleOpenMenu(event, node)}
              component={hasChildren ? "div" : NavLink}
              to={hasChildren ? undefined : `/dashboard/module/${node.ValueNode}`}
              className={
                hasChildren
                  ? undefined
                  : ({ isActive }: { isActive: boolean }) =>
                      isActive ? "active" : ""
              }
              sx={{
                borderRadius: 2,
                mb: 0.5,
                px: isOpen ? 2 : 1.5,
                py: 1.2,
                "&.active": {
                  backgroundColor: "primary.main",
                  color: "primary.contrastText",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: isOpen ? 36 : 28 }}>
                {renderMenuIcon(node.Icono)}
              </ListItemIcon>
              {isOpen && <ListItemText primary={node.NombreModulo} />}
              {isOpen && hasChildren && (
                <i className="fa-solid fa-chevron-right" aria-hidden />
              )}
            </ListItemButton>
          );

          return isOpen ? (
            <Box key={node.ValueNode}>{listItem}</Box>
          ) : (
            <Tooltip key={node.ValueNode} title={node.NombreModulo} placement="right">
              <Box>{listItem}</Box>
            </Tooltip>
          );
        })}
      </List>
    </Box>
  );

  return (
    <>
      <Drawer
        variant={isMobile ? "temporary" : "permanent"}
        open={isMobile ? mobileOpen : true}
        onClose={onClose}
        ModalProps={{ keepMounted: true }}
        sx={{
          width: isMobile ? drawerWidth : isOpen ? drawerWidth : collapsedWidth,
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: isMobile ? drawerWidth : isOpen ? drawerWidth : collapsedWidth,
            boxSizing: "border-box",
            transition: "width 0.2s",
            borderRight: "1px solid",
            borderColor: "divider",
          },
        }}
      >
        {drawerContent}
      </Drawer>

      <Popover
        open={Boolean(anchorEl && activeParent)}
        anchorEl={anchorEl}
        onClose={handleCloseMenu}
        anchorOrigin={{ vertical: "top", horizontal: "right" }}
        transformOrigin={{ vertical: "top", horizontal: "left" }}
        PaperProps={{
          onMouseLeave: handleCloseMenu,
          sx: { p: 1.5, minWidth: 240 },
        }}
        disableRestoreFocus
      >
        <Typography variant="subtitle2" sx={{ px: 1, mb: 1 }}>
          {activeParent?.NombreModulo}
        </Typography>
        <List sx={{ p: 0 }}>
          {activeParent?.children.map((child) => (
            <ListItemButton
              key={child.ValueNode}
              component={NavLink}
              to={`/dashboard/module/${child.ValueNode}`}
              className={({ isActive }: { isActive: boolean }) =>
                isActive ? "active" : ""
              }
              onClick={handleCloseMenu}
              sx={{
                borderRadius: 2,
                mb: 0.5,
                "&.active": {
                  backgroundColor: "primary.main",
                  color: "primary.contrastText",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 36 }}>
                {renderMenuIcon(child.Icono)}
              </ListItemIcon>
              <ListItemText
                primary={child.NombreModulo}
                secondary={child.ToltipNode}
              />
            </ListItemButton>
          ))}
        </List>
      </Popover>
    </>
  );
};

export default Sidebar;
