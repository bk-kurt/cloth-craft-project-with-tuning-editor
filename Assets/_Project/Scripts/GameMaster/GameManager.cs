
public class GameManager : MonoSingleton<GameManager>
{
    public Player player;
    public AudioManager audioManager;
    public SectionManager sectionManager;
    public UIManager UIManager;

    public Stats stats;
    public Preferences preferences;
}
