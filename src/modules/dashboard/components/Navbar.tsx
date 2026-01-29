import {
  AppBar,
  Avatar,
  Badge,
  Box,
  IconButton,
  Toolbar,
  Typography,
  useMediaQuery,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";

interface NavbarProps {
  onToggleSidebar: () => void;
  sidebarWidth: number;
}

/**
 * Barra superior con controles de usuario y botón para colapsar el sidebar.
 */
const Navbar = ({ onToggleSidebar, sidebarWidth }: NavbarProps) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));

  return (
    <AppBar
      position="fixed"
      color="inherit"
      elevation={1}
      sx={{
        borderBottom: "1px solid",
        borderColor: "divider",
        backgroundColor: "#fff",
        ml: isMobile ? 0 : `${sidebarWidth}px`,
        width: isMobile ? "100%" : `calc(100% - ${sidebarWidth}px)`,
      }}
    >
      <Toolbar
        sx={{
          display: "flex",
          justifyContent: "space-between",
          gap: 2,
          minHeight: { xs: 64, md: 72 },
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
          <IconButton
            aria-label="Colapsar o expandir sidebar"
            onClick={onToggleSidebar}
            edge="start"
            sx={{
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 2,
              width: 44,
              height: 44,
            }}
          >
            <i className="fa-solid fa-bars" />
          </IconButton>
          <Typography
            variant={isMobile ? "subtitle1" : "h6"}
            fontWeight={600}
            sx={{ color: "text.primary" }}
          >
            DocuArchiCore
          </Typography>
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
          <IconButton
            aria-label="Ir a inicio"
            sx={{
              border: "1px solid",
              borderColor: "divider",
              borderRadius: 2,
              width: 44,
              height: 44,
            }}
          >
            <i className="fa-solid fa-house" />
          </IconButton>
          <Badge color="primary" variant="dot" overlap="circular">
            <IconButton
              aria-label="Notificaciones"
              sx={{
                border: "1px solid",
                borderColor: "divider",
                borderRadius: 2,
                width: 44,
                height: 44,
              }}
            >
              <i className="fa-regular fa-bell" />
            </IconButton>
          </Badge>
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <Avatar sx={{ bgcolor: "primary.main", width: 36, height: 36 }}>
              MI
            </Avatar>
            {!isMobile && (
              <Typography variant="body2" fontWeight={600}>
                Miguel
              </Typography>
            )}
          </Box>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
