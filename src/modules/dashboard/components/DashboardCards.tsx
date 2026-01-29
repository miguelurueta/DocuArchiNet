import { Box, Card, CardContent, Typography } from "@mui/material";
import { useMenuItems } from "../hooks/useMenuItems";
import { getDirectAccessItems } from "../utils/menuTree";

/**
 * Tarjetas principales del dashboard con accesos directos.
 */
const DashboardCards = () => {
  const { data } = useMenuItems();
  const cards = getDirectAccessItems(data ?? []).slice(0, 3);

  return (
    <Box
      sx={{
        display: "grid",
        gridTemplateColumns: { xs: "1fr", md: "repeat(3, 1fr)" },
        gap: 3,
      }}
    >
      {cards.map((card) => (
        <Card
          key={card.ValueNode}
          elevation={0}
          sx={{
            borderRadius: 4,
            boxShadow: "0 12px 24px rgba(15, 23, 42, 0.08)",
          }}
        >
          <CardContent
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: 2,
              minHeight: 110,
            }}
          >
            <Box>
              <Typography variant="body2" color="text.secondary">
                {card.NombreModulo}
              </Typography>
              <Typography variant="h5" fontWeight={700}>
                0
              </Typography>
            </Box>
            <Box
              sx={{
                width: 56,
                height: 56,
                borderRadius: "50%",
                backgroundColor: "#E7EEF8",
                display: "grid",
                placeItems: "center",
                color: "primary.main",
              }}
            >
              <i className={card.Icono || "fa-solid fa-layer-group"} />
            </Box>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
};

export default DashboardCards;
