

// import { Layout } from "antd";
// import { Outlet } from "react-router-dom";
// import { useEffect, useState } from "react";
// import { useMediaQuery } from "@mui/material";
// import { useTheme } from "@mui/material/styles";

// import Sidebar from "../components/Sidebar";
// import Navbar from "../components/Navbar";
// import { useDashboardMenu } from "../hooks/useDashboardMenu";
// import type { MenuNode } from "../types/menu";
// import { useDashboardMetrics } from "../hooks/useDashboardMetrics";
// import DashboardMetricsProvider from "../context/DashboardMetricsProvider";


// export type DashboardOutletContext = {
//   menuTree: MenuNode[];
// };

// const { Content } = Layout;

// export default function DashboardLayout() {
//   const { menuTree, isLoading } = useDashboardMenu();

//   const theme = useTheme();
//   const isMobile = useMediaQuery(theme.breakpoints.down("md"));

//   const [collapsed, setCollapsed] = useState(false);

//   /**
//    * 📱 Auto-ocultar sidebar según tamaño de pantalla
//    * - Mobile / Tablet → oculto
//    * - Desktop → visible
//    */
//   useEffect(() => {
//     setCollapsed(isMobile);
//   }, [isMobile]);

//   return (
//     <DashboardMetricsProvider>
//     <Layout
//       style={{
//         minHeight: "100vh",
//         width: "100vw",
//         overflow: "hidden",
//       }}
//     >
//       {/* SIDEBAR */}
//       <Sidebar
//         collapsed={collapsed}
//         onCollapse={setCollapsed}
//         menuTree={menuTree}
//         isLoading={isLoading}
//       />

//       {/* CONTENEDOR PRINCIPAL */}
//       <Layout
//         style={{
//           flex: 1,
//           minWidth: 0, // 🔑 crítico para flex layouts
//           width: "100%",
//         }}
//       >
//         {/* NAVBAR */}
//         <Navbar
//           collapsed={collapsed}
//           onToggle={() => {
//             // 🔐 Solo permitir toggle manual en desktop
//             if (!isMobile) {
//               setCollapsed((v) => !v);
//             }
//           }}
//         />

//         {/* CONTENIDO */}
//         <Content
//           style={{
//             flex: 1,
//             width: "100%",
//             padding: 20,
//             overflowY: "auto",
//             overflowX: "hidden",
//             background: "#f5f6fa",
//           }}
//         >
//           <Outlet context={{ menuTree } satisfies DashboardOutletContext} />
//         </Content>
//       </Layout>
//     </Layout>
//     </DashboardMetricsProvider>
//   );
// }
import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import { useEffect, useState } from "react";
import { useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";

import Sidebar from "../components/Sidebar";
import Navbar from "../components/Navbar";
import { useDashboardMenu } from "../hooks/useDashboardMenu";
import { useDashboardMetrics } from "../hooks/useDashboardMetrics";
import type { MenuNode } from "../types/menu";

export type DashboardOutletContext = {
  menuTree: MenuNode[];
  metricMap: Map<number, number>;
  metricsLoading: boolean;
};

const { Content } = Layout;

export default function DashboardLayout() {
  const { menuTree, isLoading } = useDashboardMenu();
  const { metricMap, isLoading: metricsLoading } = useDashboardMetrics();

  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));
  const [collapsed, setCollapsed] = useState(false);

  useEffect(() => {
    setCollapsed(isMobile);
  }, [isMobile]);

  return (
    <Layout style={{ minHeight: "100vh", width: "100vw", overflow: "hidden" }}>
      <Sidebar
        collapsed={collapsed}
        onCollapse={setCollapsed}
        menuTree={menuTree}
        metricMap={metricMap}
        isLoading={isLoading}
      />

      <Layout style={{ flex: 1, minWidth: 0, width: "100%" }}>
        <Navbar
          collapsed={collapsed}
          onToggle={() => {
            if (!isMobile) {
              setCollapsed((v) => !v);
            }
          }}
        />

        <Content
          style={{
            flex: 1,
            width: "100%",
            padding: 20,
            overflowY: "auto",
            overflowX: "hidden",
            background: "#f5f6fa",
          }}
        >
          <Outlet
            context={{
              menuTree,
              metricMap,
              metricsLoading,
            } satisfies DashboardOutletContext}
          />
        </Content>
      </Layout>
    </Layout>
  );
}
