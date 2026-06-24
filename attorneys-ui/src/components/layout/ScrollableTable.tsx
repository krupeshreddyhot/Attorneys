import { Paper, TableContainer } from "@mui/material";
import type { ReactNode } from "react";

type ScrollableTableProps = {
  children: ReactNode;
  noPaper?: boolean;
};

const ScrollableTable = ({ children, noPaper = false }: ScrollableTableProps) => {
  const table = (
    <TableContainer
      sx={{
        overflowX: "auto",
        WebkitOverflowScrolling: "touch",
        maxWidth: "100%",
      }}
    >
      {children}
    </TableContainer>
  );

  if (noPaper) return table;

  return <Paper sx={{ overflow: "hidden" }}>{table}</Paper>;
};

export default ScrollableTable;
