import { useState } from "react";
import SectionHeader from "../components/SectionHeader";
import CardComponent from "../components/CardComponent";
import { useOutletContext } from "react-router";
import type { DashboardOutletContext } from "../components/DashboardLayout";

export default function DashboardHome() {
  const { menuTree, metricMap, metricsLoading } =
    useOutletContext<DashboardOutletContext>();

  const [open, setOpen] = useState(true);

  return (
    <>
      <SectionHeader
        title="Accesos Directos"
        open={open}
        onToggle={() => setOpen((v) => !v)}
      />
      <CardComponent
        menuTree={menuTree}
        metricMap={metricMap}
        open={open}
        loading={metricsLoading}
      />
    </>
  );
}



