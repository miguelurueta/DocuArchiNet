import { Box, Toolbar, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useState } from "react";
import Navbar from "./Navbar";
import Sidebar from "./Sidebar";

interface DashboardLayoutProps {
  children: React.ReactNode;
}

const drawerWidth = 260;
const collapsedWidth = 78;

/**
 * Layout principal del dashboard con navbar y sidebar colapsable.
 */
const DashboardLayout = ({ children }: DashboardLayoutProps) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [mobileOpen, setMobileOpen] = useState(false);
  const sidebarWidth = isMobile ? 0 : isSidebarOpen ? drawerWidth : collapsedWidth;

  const handleToggleSidebar = () => {
    if (isMobile) {
      setMobileOpen((prev) => !prev);
      return;
    }
    setIsSidebarOpen((prev) => !prev);
  };

  const handleCloseMobile = () => setMobileOpen(false);

  return (
    <Box sx={{ display: "flex", minHeight: "100vh", bgcolor: "#F7F8FB" }}>
      <Navbar onToggleSidebar={handleToggleSidebar} sidebarWidth={sidebarWidth} />
      <Sidebar
        isOpen={isSidebarOpen}
        isMobile={isMobile}
        mobileOpen={mobileOpen}
        onClose={handleCloseMobile}
      />

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          ml: { md: `${sidebarWidth}px` },
          px: { xs: 2, md: 4 },
          py: { xs: 3, md: 4 },
        }}
      >
        <Toolbar />
        {children}
      </Box>
    </Box>
  );
};

export default DashboardLayout;
