# Details of performed tests

## Homogeneous networks

### Simulation prameters

<table>
<thead>
  <tr>
    <th rowspan="2">Nodes<br> number</th>
    <th colspan="3">Standard FBE</th>
    <th colspan="4">Fixed-muting FBE</th>
    <th colspan="5">Random-muting FBE</th>
    <th colspan="3">Floating FBE</th>
    <th colspan="6">Deterministic-backoff FBE</th>
    <th colspan="4">Enhanced/Greedy-enhanced/BITR FBE</th>
  </tr>
  <tr>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>N</th>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>N</th>
    <th>M</th>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>Beta</th>
    <th>Threshold</th>
    <th>max</th>
    <th>FFP</th>
    <th>COT</th>
    <th>Shift</th>
    <th>Q</th>
  </tr>
</thead>
<tbody>
  <tr>
    <td>2</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491)\times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491)\times (N_{seq} - 1)$</td>
    <td>0</td>
    <td>1000</td>
    <td>491</td>
    <td>$250 \times (N_{seq} - 1)$</td>
    <td>2</td>
    <td>2</td>
    <td>1000</td>
    <td>500</td>
    <td>$500 \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>900</td>
    <td>0</td>
    <td>2</td>
    <td>2</td>
    <td>4</td>
    <td>2000</td>
    <td>1900</td>
    <td>0</td>
    <td>4</td>
  </tr>
  <tr>
    <td>4</td>
    <td>2000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} - 1)$</td>
    <td>1</td>
    <td>1500</td>
    <td>491</td>
    <td>$250 \times (N_{seq} - 1)$</td>
    <td>2</td>
    <td>2</td>
    <td>1000</td>
    <td>500</td>
    <td>$500 \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>900</td>
    <td>0</td>
    <td>2</td>
    <td>2</td>
    <td>8</td>
    <td>2000</td>
    <td>1900</td>
    <td>0</td>
    <td>8</td>
  </tr>
  <tr>
    <td>8</td>
    <td>4000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} - 1)$</td>
    <td>3</td>
    <td>2500</td>
    <td>491</td>
    <td>$250 \times (N_{seq} - 1)$</td>
    <td>2</td>
    <td>2</td>
    <td>2000</td>
    <td>500</td>
    <td>$500 \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>900</td>
    <td>0</td>
    <td>2</td>
    <td>2</td>
    <td>16</td>
    <td>2000</td>
    <td>1900</td>
    <td>0</td>
    <td>16</td>
  </tr>
  <tr>
    <td>16</td>
    <td>8000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} -1)$</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq} - 1)$</td>
    <td>7</td>
    <td>4500</td>
    <td>491</td>
    <td>$250 \times (N_{seq} - 1)$</td>
    <td>2</td>
    <td>2</td>
    <td>2000</td>
    <td>500</td>
    <td>$500 \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>900</td>
    <td>0</td>
    <td>2</td>
    <td>2</td>
    <td>32</td>
    <td>2000</td>
    <td>1900</td>
    <td>0</td>
    <td>32</td>
  </tr>
  <tr>
    <td>32</td>
    <td>10000</td>
    <td>303</td>
    <td>$(9+303) \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>491</td>
    <td>$(9+491) \times (N_{seq - 1})$</td>
    <td>15</td>
    <td>8500</td>
    <td>491</td>
    <td>$250 \times (N_{seq} - 1)$</td>
    <td>2</td>
    <td>2</td>
    <td>3000</td>
    <td>500</td>
    <td>$500 \times (N_{seq} - 1)$</td>
    <td>1000</td>
    <td>900</td>
    <td>0</td>
    <td>2</td>
    <td>2</td>
    <td>64</td>
    <td>2000</td>
    <td>1900</td>
    <td>0</td>
    <td>64</td>
  </tr>
</tbody>
</table>

# Symbols definitions

- $N_{seq}$ - node sequence number (starting from 1). 