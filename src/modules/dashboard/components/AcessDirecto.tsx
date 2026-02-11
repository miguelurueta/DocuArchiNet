import { Typography } from "@mui/material";
import ExpandCircleDownIcon from '@mui/icons-material/ExpandCircleDown';
import styles from "../style/acessdirecto.module.css";

interface AcessDirectoProps {
  title: string;
  open: boolean;
  onToggle: () => void;
}

export default function AcessDirecto({
  title,
  open,
  onToggle,
}: AcessDirectoProps) {
  return (
    <div className={styles.header} onClick={onToggle}>
      <Typography className={styles.title}>{title}</Typography>

      <ExpandCircleDownIcon fontSize="medium"
        className={`${styles.icon} ${!open ? styles.rotated : ""}`}
      />
    </div>
  );
}
