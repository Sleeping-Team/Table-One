using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Customer : NetworkBehaviour
{
    public CustomerState State => _customerState;
    
    CustomerState _customerState = CustomerState.WaitingTable;

    [SerializeField]private TablePosition _table;
    public int Quantity = 1;
    
    public enum CustomerState
    {
        WaitingTable, //Stand at entrance waiting for table
        OnTable, // Standby at Table
        Roaming
    }
    
    private void OnEnable()
    {
        _customerState = CustomerState.WaitingTable;
    }
    
    public void SetState(CustomerState state)
    {
        _customerState = state;
    }

    public void AssignTable()
    {
        AssignTableServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AssignTableServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Assign Table");
        
        _customerState = CustomerState.OnTable;

        if (_table == null)
        {
            _table = FloorPlan.Instance.SearchVacantTable(Quantity);
        }
        
        TableOrder theTable = _table.GetComponent<TableOrder>();
        TablePosition tableDetail = _table.GetComponent<TablePosition>();
        
        theTable.ChangeState(TableOrder.TableState.Ordering);
        theTable.SetStatus(true);
        theTable.AssignCustomer(this);
        
        transform.SetParent(_table.transform);
        tableDetail.AssignObject(TablePosition.ObjectOnFocus.Chair, this.transform);

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        
        AssignTableClientRpc(name);
    }

    [ClientRpc]
    public void AssignTableClientRpc(string target)
    {
        GameObject focus = GameObject.Find(target);
        focus.GetComponent<Customer>().SetState(CustomerState.OnTable);
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (_table == null & transform.parent != null) _table = transform.parent.GetComponent<TablePosition>();

        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }

    IEnumerator EatCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}

