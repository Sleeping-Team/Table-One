using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Customer : NetworkBehaviour, IInteractable
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
        
        TablePosition tableDetail = _table.GetComponent<TablePosition>();

        TableSetupClientRpc(_table.name, name);

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

    [ClientRpc]
    public void TableSetupClientRpc(string target, string customer)
    {
        TableOrder focus = GameObject.Find(target).GetComponent<TableOrder>();
        focus.ChangeState(TableOrder.TableState.Ordering);
        focus.SetStatus(true);
        focus.AssignCustomer(GameObject.Find(customer).GetComponent<Customer>());
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (_table == null & transform.parent != null) _table = transform.parent.GetComponent<TablePosition>();
        
        name = $"{name}_{NetworkObject.NetworkObjectId}";

        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }

    IEnumerator EatCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public void OnEnter()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-1.03f, 1f);
        interactionIcon.DOFade(1f, 1f);
    }

    public void OnExit()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-0.74f, 1f);
        interactionIcon.DOFade(0f, 0.5f);
    }
}

