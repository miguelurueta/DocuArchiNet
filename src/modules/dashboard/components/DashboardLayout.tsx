import { Drawer, Grid, Layout } from "antd";
import { Outlet } from "react-router-dom";
import { useEffect, useRef, useState } from "react";

import Sidebar from "../components/Sidebar";
import Navbar from "../components/Navbar";
import { useDashboardMenu } from "../hooks/useDashboardMenu";
import { useDashboardMetrics } from "../hooks/useDashboardMetrics";
import type { MenuNode } from "../types/menu";
import { useAppErrorNotifier } from "../../../shared/hooks/useAppErrorNotifier";
import type { AppError } from "../../../shared/errors/AppError";

export type DashboardOutletContext = {
  menuTree: MenuNode[];
  metricMap: Map<number, number>;
  metricsLoading: boolean;
};

const { Content } = Layout;
const { useBreakpoint } = Grid;
const MOBILE_DRAWER_BELOW = 900;

const resolveIsNarrowViewport = () => {
  if (typeof window === "undefined") {
    return false;
  }

  return window.innerWidth < MOBILE_DRAWER_BELOW;
};

export default function DashboardLayout() {
  const { menuTree, isLoading, error } = useDashboardMenu();
  const { metricMap, isLoading: metricsLoading } = useDashboardMetrics();
  const notifyError = useAppErrorNotifier();
  const lastNotifiedErrorRef = useRef<string | null>(null);

  const screens = useBreakpoint();
  const [isNarrowViewport, setIsNarrowViewport] = useState(resolveIsNarrowViewport);
  const isMobile = !screens.md || isNarrowViewport;
  const isTablet = Boolean(screens.md) && !screens.xl;
  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);

  useEffect(() => {
    if (typeof window === "undefined") {
      return undefined;
    }

    const onResize = () => {
      setIsNarrowViewport(resolveIsNarrowViewport());
    };

    window.addEventListener("resize", onResize);
    onResize();

    return () => {
      window.removeEventListener("resize", onResize);
    };
  }, []);

  useEffect(() => {
    setCollapsed(isMobile || (!isMobile && isTablet));
    if (!isMobile) {
      setDrawerOpen(false);
    }
  }, [isMobile, isTablet]);

  useEffect(() => {
    if (!error) {
      lastNotifiedErrorRef.current = null;
      return;
    }

    const fingerprint = `${error.name}:${error.message}`;
    if (lastNotifiedErrorRef.current === fingerprint) return;

    const dashboardMenuError: AppError = {
      source: "api",
      severity: "error",
      message: "No se pudo cargar el menú del dashboard.",
      details: error,
    };

    notifyError(dashboardMenuError);
    lastNotifiedErrorRef.current = fingerprint;
  }, [error, notifyError]);

  return (
    <Layout style={{ minHeight: "100vh", width: "100vw", overflow: "hidden" }}>
      {!isMobile ? (
        <Sidebar
          collapsed={collapsed}
          onCollapse={setCollapsed}
          menuTree={menuTree}
          metricMap={metricMap}
          isLoading={isLoading}
        />
      ) : null}

      <Layout style={{ flex: 1, minWidth: 0, width: "100%" }}>
        <Navbar
          collapsed={collapsed}
          isMobile={isMobile}
          onToggle={() => {
            if (isMobile) {
              setDrawerOpen(true);
              return;
            }

            setCollapsed((value) => !value);
          }}
        />

        <Content
          style={{
            flex: 1,
            width: "100%",
            padding: "10px 12px 8px",
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

      {isMobile ? (
        <Drawer
          placement="left"
          open={drawerOpen}
          size="default"
          onClose={() => setDrawerOpen(false)}
          styles={{ body: { padding: 0 } }}
          destroyOnHidden={false}
        >
          <Sidebar
            collapsed={false}
            onCollapse={() => undefined}
            menuTree={menuTree}
            metricMap={metricMap}
            isLoading={isLoading}
            renderInDrawer
          />
        </Drawer>
      ) : null}
    </Layout>
  );
}
