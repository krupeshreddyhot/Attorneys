import { useEffect, useState } from "react";
import { Table, TableBody, TableCell, TableHead, TableRow } from "@mui/material";
import api from "../../services/api";
import PageHeader from "../../components/layout/PageHeader";
import ScrollableTable from "../../components/layout/ScrollableTable";

type Account = {
  caseNo: string;
  totalAmount: number;
  paidTotal: number;
  balance: number;
};

const AccountsPage = () => {
  const [accounts, setAccounts] = useState<Account[]>([]);

  useEffect(() => {
    api.get<Account[]>("/accounts").then((res) => setAccounts(res.data));
  }, []);

  return (
    <>
      <PageHeader title="Accounts" />
      <ScrollableTable>
        <Table size="small" sx={{ minWidth: 360 }}>
          <TableHead>
            <TableRow>
              <TableCell>Case No</TableCell>
              <TableCell align="right">Total</TableCell>
              <TableCell align="right">Paid</TableCell>
              <TableCell align="right">Balance</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {accounts.map((a) => (
              <TableRow key={a.caseNo}>
                <TableCell sx={{ fontWeight: 600 }}>{a.caseNo}</TableCell>
                <TableCell align="right">{a.totalAmount.toFixed(2)}</TableCell>
                <TableCell align="right">{a.paidTotal.toFixed(2)}</TableCell>
                <TableCell align="right">{a.balance.toFixed(2)}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </ScrollableTable>
    </>
  );
};

export default AccountsPage;
