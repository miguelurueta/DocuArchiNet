// import { useMemo } from "react";
// import Grid from "@mui/material/Grid";
// import Collapse from "@mui/material/Collapse";
// import styles from "../style/container.module.css";
// import BuildCard from "../utils/BuildCard";
// import type { MenuNode } from "../types/menu";

// interface CardComponentProps {
//   menuTree: MenuNode[];
//   open?: boolean;
// }

// export default function CardComponent({ menuTree, open = true }: CardComponentProps) {
//   const accesosDirectos = useMemo(() => {
//     return BuildCard(menuTree).sort((a, b) => (a.Orden ?? 0) - (b.Orden ?? 0));
//   }, [menuTree]);

//   if (!accesosDirectos.length) return null;

//   return (
//     <Collapse in={open} timeout="auto" unmountOnExit>
//       <Grid container spacing={2} sx={{ width: "100%", maxWidth: "100%", m: 0 }}>
//         {accesosDirectos.map((item) => (
//           <Grid key={item.IdMenuPrincipal} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
//             <div className={styles.card} role="button" tabIndex={0} aria-label={item.NombreModulo}>
//               <div className={styles.iconContainer}>
//                 <i className={item.Icono || "fa-solid fa-circle"} aria-hidden="true" />
//               </div>

//               <div className={styles.textContainer}>
//                 <span className={styles.title}>{item.NombreModulo}</span>
//                 {item.ToltipNode && <span className={styles.subtitle}>{item.ToltipNode}</span>}
//               </div>
//             </div>
//           </Grid>
//         ))}
//       </Grid>
//     </Collapse>
//   );
// }
// import { useMemo } from "react";
// import Grid from "@mui/material/Grid";
// import Collapse from "@mui/material/Collapse";
// import Badge from "@mui/material/Badge";
// import { useNavigate } from "react-router-dom";
// import styles from "../style/container.module.css";
// import BuildCard from "../utils/BuildCard";
// import type { MenuNode } from "../types/menu";
// import type { DashboardMetric } from "../types/dashboardMetrics";

// interface CardComponentProps {
//   menuTree: MenuNode[];
//   metrics?: DashboardMetric[];
//   open?: boolean;
//   loading?: boolean;
// }


// export default function CardComponent({
//   menuTree,
//   metrics = [],
//   open = true,
// }: CardComponentProps) {
//   const navigate = useNavigate();

//    const accesosDirectos = useMemo(() => {
//     return BuildCard(menuTree).sort(
//       (a, b) => (a.Orden ?? 0) - (b.Orden ?? 0)
//     );
//   }, [menuTree]);

//   const metricMap = useMemo(() => {
//     const map = new Map<number, number>();
//     metrics.forEach(m => map.set(m.nodeId, m.pendingCount));
//     return map;
//   }, [metrics]);

//   //if (!accesosDirectos.length) return null;
//   // if (!menuTree || menuTree.length === 0) {
//   // return null; // aún no hay menú
//   // }

//   //console.log(accesosDirectos);
//   //console.log(metricMap);
//   return (
//     <Collapse in={open} timeout="auto" unmountOnExit>
//       <Grid container spacing={2} sx={{ width: "100%", m: 0 }}>
//         {accesosDirectos.map(item => {
//           const pending = metricMap.get(item.IdMenuPrincipal) ?? 0;

//           return (
//             <Grid
//               key={item.IdMenuPrincipal}
//               size={{ xs: 12, sm: 6, md: 4, lg: 3 }}
//             >
//               <div
//                 className={styles.card}
//                 role="button"
//                 tabIndex={0}
//                 aria-label={item.NombreModulo}
//                 onClick={() => item.UrlNode && navigate(item.UrlNode)}
//                 onKeyDown={(e) => {
//                   if ((e.key === "Enter" || e.key === " ") && item.UrlNode) {
//                     e.preventDefault();
//                     navigate(item.UrlNode);
//                   }
//                 }}
//               >
//                 {/* ICONO + BADGE */}
//                 <Badge
//                   color="error"
//                   badgeContent={pending > 0 ? pending : undefined}
//                   overlap="circular"
//                 >
//                   <div className={styles.iconContainer}>
//                     <i
//                       className={item.Icono || "fa-solid fa-circle"}
//                       aria-hidden="true"
//                     />
//                   </div>
//                 </Badge>

//                 {/* TEXTO */}
//                 <div className={styles.textContainer}>
//                   <span className={styles.title}>
//                     {item.NombreModulo}
//                   </span>
//                 </div>
//               </div>
//             </Grid>
//           );
//         })}
//       </Grid>
//     </Collapse>
//   );
// }
// import { useMemo } from "react";
// import { useNavigate } from "react-router-dom";
// import Grid from "@mui/material/Grid";
// import Collapse from "@mui/material/Collapse";
// import Badge from "@mui/material/Badge";

// import styles from "../style/container.module.css";
// import BuildCard from "../utils/BuildCard";
// import type { MenuNode } from "../types/menu";
// import type { DashboardMetric } from "../types/dashboardMetrics";
// import Tooltip from "@mui/material/Tooltip";

// interface CardComponentProps {
//   menuTree: MenuNode[];
//   metrics?: DashboardMetric[];
//   open?: boolean;
// }

// export default function CardComponent({
//   menuTree,
//   metrics = [],
//   open = true,
// }: CardComponentProps) {
//   const navigate = useNavigate();

//   /** 🔑 Métricas por nodo */
//   const metricMap = useMemo(() => {
//     const map = new Map<number, number>();
//     metrics.forEach((m) => map.set(m.nodeId, m.pendingCount));
//     return map;
//   }, [metrics]);

//   /** 🔑 Accesos directos */
//   const accesosDirectos = useMemo(() => {
//     if (!Array.isArray(menuTree) || menuTree.length === 0) return [];
//     return BuildCard(menuTree).sort(
//       (a, b) => (a.Orden ?? 0) - (b.Orden ?? 0)
//     );
//   }, [menuTree]);

//   if (!accesosDirectos.length) return null;

//   return (
//     <Collapse in={open} timeout="auto" unmountOnExit>
//       <Grid container spacing={2} sx={{ width: "100%", margin: 0 }}>
//         {accesosDirectos.map((item) => {
//           const pending = metricMap.get(item.IdMenuPrincipal) ?? 0;

//           return (
//             <Grid
//               key={item.IdMenuPrincipal}
//               item
//               xs={12}
//               sm={6}
//               md={4}
//               lg={3}
//             >
//               <div
//                 className={styles.card}
//                 role="button"
//                 tabIndex={0}
//                 aria-label={item.NombreModulo}
//                 onClick={() => item.UrlNode && navigate(item.UrlNode)}
//                 onKeyDown={(e) => {
//                   if (
//                     (e.key === "Enter" || e.key === " ") &&
//                     item.UrlNode
//                   ) {
//                     e.preventDefault();
//                     navigate(item.UrlNode);
//                   }
//                 }}
//               >
//                 {/* ICONO + BADGE */}
//                 <Badge
//                   color="error"
//                   badgeContent={pending}
//                   showZero
//                   overlap="circular"
//                 >
//                   <div className={styles.iconContainer}>
//                     <i
//                       className={item.Icono || "fa-solid fa-circle"}
//                       aria-hidden="true"
//                     />
//                   </div>
//                 </Badge>

//                 {/* TEXTO */}
//                 {/* <div className={styles.textContainer}>
//                   <span className={styles.title}>
//                     {item.NombreModulo}
//                   </span>
//                 </div> */}
//                 <div className={styles.textContainer}>
//                   <Tooltip
//                     title={item.ToltipNode}
//                     placement="top"
//                     arrow
//                   >
//                     <span className={styles.title}>
//                       {item.NombreModulo}
//                     </span>
//                   </Tooltip>
//                 </div>

//               </div>
//             </Grid>
//           );
//         })}
//       </Grid>
//     </Collapse>
//   );
// }
import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import Grid from "@mui/material/GridLegacy";
import Collapse from "@mui/material/Collapse";
import Badge from "@mui/material/Badge";
import Tooltip from "@mui/material/Tooltip";

import styles from "../style/container.module.css";
import BuildCard from "../utils/BuildCard";
import type { MenuNode } from "../types/menu";

interface CardComponentProps {
  menuTree: MenuNode[];
  metricMap: Map<number, number>;
  open?: boolean;
  loading?: boolean;
}

export default function CardComponent({
  menuTree,
  metricMap,
  open = true,
}: CardComponentProps) {
  const navigate = useNavigate();

  const accesosDirectos = useMemo(() => {
    if (!Array.isArray(menuTree) || menuTree.length === 0) return [];
    return BuildCard(menuTree).sort(
      (a, b) => (a.Orden ?? 0) - (b.Orden ?? 0)
    );
  }, [menuTree]);

  if (!accesosDirectos.length) return null;

  return (
    <Collapse in={open} timeout="auto" unmountOnExit>
      <Grid container spacing={2} sx={{ width: "100%", margin: 0 }}>
        {accesosDirectos.map((item) => {
          const pending = metricMap.get(item.IdMenuPrincipal) ?? 0;

          return (
            <Grid key={item.IdMenuPrincipal} item xs={12} sm={6} md={4} lg={3}>
              <div
                className={styles.card}
                role="button"
                tabIndex={0}
                aria-label={item.NombreModulo}
                onClick={() => item.UrlNode && navigate(item.UrlNode)}
              >
                <Badge
                  color="error"
                  badgeContent={pending}
                  showZero
                  overlap="circular"
                >
                  <div className={styles.iconContainer}>
                    <i
                      className={item.Icono || "fa-solid fa-circle"}
                      aria-hidden="true"
                    />
                  </div>
                </Badge>

                <div className={styles.textContainer}>
                  <Tooltip title={item.ToltipNode} placement="top" arrow>
                    <span className={styles.title}>
                      {item.NombreModulo}
                    </span>
                  </Tooltip>
                </div>
              </div>
            </Grid>
          );
        })}
      </Grid>
    </Collapse>
  );
}
