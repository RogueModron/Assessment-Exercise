import {
  Box,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  styled,
  tableCellClasses,
} from "@mui/material";
import { FocusEvent, useEffect, useRef, useState } from "react";
import { js2xml } from "xml-js";
import TextDialog from "../components/TextDialog";

interface CustomerListQuery {
  id: number;
  name: string;
  address: string;
  email: string;
  phone: string;
  iban: string;
  customerCategory?: {
    code: string;
    description: string;
  }
}

export default function CustomerListPage() {
  const [list, setList] = useState<CustomerListQuery[]>([]);

  function loadData(searchText?: string) {
    const queryParameters = searchText ? `?searchText=${searchText}` : "";
    fetch(`/api/customers/list${queryParameters}`)
      .then((response) => {
        return response.json();
      })
      .then((data) => {
        setList(data as CustomerListQuery[]);
      });
  };

  useEffect(loadData, []);

  const searchTextRef = useRef<string>("");

  function onSearchClick() {
    loadData(searchTextRef.current);
  }

  function onSearchTextBlur(e: FocusEvent<HTMLInputElement>) {
    searchTextRef.current = e.target?.value;
  }

  const [openTextDialog, setOpenTextDialog] = useState(false);
  const [exportXmlResult, setExportXmlResult] = useState("");

  function handleCloseTextDialog() {
    setOpenTextDialog(false);
  }

  function onExportXmlClick() {
    const data = {
      customers: {
        customer: list
      }
    };
    const options = { compact: true, spaces: 4 };
    const result = js2xml(data, options);
    setExportXmlResult(result);
    setOpenTextDialog(true);
  }

  return (
    <>
      <Typography variant="h4" sx={{ textAlign: "center", mt: 4, mb: 4 }}>
        Customers
      </Typography>

      <Box sx={{ p: "2px 4px", display: "flex", alignItems: "center", mb: 2 }}>
        <TextField label="Search text" type="search"
          defaultValue=""
          onBlur={onSearchTextBlur} />
        <Button variant="outlined" onClick={onSearchClick} sx={{ ml: 1 }}>Search</Button>
        <Button variant="outlined" onClick={onExportXmlClick} sx={{ ml: 1 }}>Export XML</Button>
      </Box>

      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <StyledTableHeadCell>Name</StyledTableHeadCell>
              <StyledTableHeadCell>Address</StyledTableHeadCell>
              <StyledTableHeadCell>Email</StyledTableHeadCell>
              <StyledTableHeadCell>Phone</StyledTableHeadCell>
              <StyledTableHeadCell>Iban</StyledTableHeadCell>
              <StyledTableHeadCell>Category</StyledTableHeadCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {list.map((row) => (
              <TableRow
                key={row.id}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell>{row.name}</TableCell>
                <TableCell>{row.address}</TableCell>
                <TableCell>{row.email}</TableCell>
                <TableCell>{row.phone}</TableCell>
                <TableCell>{row.iban}</TableCell>
                <TableCell>{row.customerCategory?.description}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TextDialog
        onClose={handleCloseTextDialog}
        open={openTextDialog}
        text={exportXmlResult} />
    </>
  );
}

const StyledTableHeadCell = styled(TableCell)(({ theme }) => ({
  [`&.${tableCellClasses.head}`]: {
    backgroundColor: theme.palette.primary.light,
    color: theme.palette.common.white,
  },
}));
