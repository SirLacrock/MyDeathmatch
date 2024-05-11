using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class AIController : MonoBehaviour
{
    public enum EState  // Criacao do enumarator
    {
        Idle,
        Patrolling,
        Chasing,
        Shooting,
        RunningAway,
        Dead
    }

    [SerializeField] private Transform[] waypoints;     // Pontos onde a ia vai se locomover
    [SerializeField] private BoxCollider baseArea;      // Area da base (FALTA IMPLEMENTAR)
    private NavMeshAgent agent;
    private Transform player = null;
    private EState currentState;

    private void Start()
    {  
        agent = this.GetComponent<NavMeshAgent>();      // Salva a variavel nav mesh agent

        player = GameObject.FindGameObjectWithTag("Player").transform;      // Salva o transform do jogador
    
        UpdateState(EState.Patrolling);     // Comeca como patrulhando
    }

    private void UpdateState(EState state)      // Metodo que altera o estado
    {
        currentState = state;   // Atualiza o estado atual

        switch(currentState)   // Implementa metodos dependendo do estado atual
        {
            case EState.Dead:
                Debug.Log("Morrendo");
                OnDead();
                break;
            case EState.RunningAway:
                Debug.Log("Fugindo");
                OnRunningAway();
                break;
            case EState.Shooting:
                Debug.Log("Atirando");
                OnShooting();
                break;
            case EState.Chasing:
                Debug.Log("Perseguindo");
                StartCoroutine(OnChasing());
                break;
            case EState.Patrolling:
                Debug.Log("Patrulhando");
                StartCoroutine(OnPatrolling());
                break;
            case EState.Idle:
                Debug.Log("Idle");
                StartCoroutine(OnIdle());
                break;
            default: Debug.Log("Sem estado definido!");
                break;
        }
    }

    private IEnumerator OnIdle()
    {         
        while (!IsPlayerSeen(10f))  // Checa se viu o jogador a cada 0.1 segundo
        {
            yield return new WaitForSeconds(.1f);
        }
        UpdateState(EState.Chasing);
    }

    private IEnumerator OnPatrolling()
    {
        if (waypoints != null)      // Checa se tem ponto para ir
        {
            while (currentState == EState.Patrolling)   // Enquanto estiver patrulhando...
            {
                yield return new WaitForSeconds(2f);    // Delay de 2 segundos antes

                int randPoint = Random.Range(0, waypoints.Length);      // Escolhe destino randomico e defini como destino
                agent.SetDestination(waypoints[randPoint].position);

                while (!InDestination())    // Enquanto nao chega no destino...
                {
                    if (IsPlayerSeen(10f))      // Checa se viu o jogador a cada 0.1 segundo
                    {
                        UpdateState(EState.Chasing);    // Passa para estado de perseguindo se viu jogador
                        break;
                    }
                    yield return new WaitForSeconds(.1f);
                }

                if (currentState != EState.Patrolling) { break; }   // Quebra o loop se ja nao mais tiver patrulhando

                Debug.Log("Chegou ao destino");
            }          
        }
        else    // Se nao tiver pontos para ir, entra no estado de idle
        {
            Debug.Log("Nao ha waypoints!");
            UpdateState(EState.Idle);
        }
    }

    private IEnumerator OnChasing()
    {
        agent.SetDestination(player.position);      // Defini posicao do jogador como destino

        while (!InDestination())    // Fica definindo a posicao do jogador como destino a cada 0.1 segundo
        {
            agent.SetDestination(player.position);      
            yield return new WaitForSeconds(.1f);
        } 
    }

    private void OnShooting()
    {
        // IMPLEMENTE AQUI O METODO DE ATIRAR
    }

    private void OnRunningAway()
    {
        // IMPLEMENTE AQUI O METODO DE FUGIR
    }

    private void OnDead()
    {
        // IMPLEMENTE AQUI O METODO DE MORRER
    }

    private bool InDestination()
    {
        // Retorna verdade se distancia que falta for menor que a distancia de parar e se nao ja tiver caminho a percorrer
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    private bool IsPlayerSeen(float detectionRange)
    {
        // Retorna verdade se a distancia entre esta ia e o jogador for menor que a area de deteccao
        if (Vector3.Distance(player.position, this.transform.position) < detectionRange) {  return true; } 
        return false;
    }
}
