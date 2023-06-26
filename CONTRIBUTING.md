## How to contribute

### Git-Related

#### **Did you find a bug?** / **Do you want to suggest something?**

- Create an issue at [this issue page](https://github.com/AndreasLrx/LaFlammeDivain/issues).

#### **Do you want to create a branch?**

- Your branch name should be formatted as `fix/<ISSUENUMBER>-<TITLE>` for bug fixes or `feature/<ISSUENUMBER>-<TITLE>` for features, example: `fix/4221-infinite-loop`.

#### **Rebase or Merge strategy ?**

- Use the rebase strategy instead of the merge.

- PR with merge commits will not be validated.

#### **Do you want to fix an issue?**

- Assigns yourself to the issue

- Create a branch

- Implement your features of fixes in it.

- Submit a [pull request](https://github.com/AndreasLrx/LaFlammeDivain/pulls).

- Once validated, rebase and merge to PR to `main` and remove the source branch (with `git branch -D <branch_name>`).

#### **How to title commits?**

- Follow the guidelines at https://cbea.ms/git-commit/

- Use imperative tense (avoid past tense).

- The title of the commit must be a summuary of the content and not be too long (less than 50 characters).

- Prefer putting detailed informations inside the commit's description.

- Include the issue(s) linked to the commit in the commit description in the following format:
  - `Linked: #42` if the issue is not closed by the commit. 
  - `Close #42` if the commit close the issue.

- Example:

  ```sh
  $> git commit -m 'Fix infinite loop when pressing Alt-F4

  This was caused by a missing check in the event loop
  The program now checks when the window is set to close

  Close #42'
  ```

---

### **DOs and DONTs**

- :x: **DONT**: Push to the `main` branch for any reason, please submit a PR instead.

- :x: **DONT**: Create a branch with your username as the title

- :heavy_check_mark: **DO**: Commit often! allows everyone to see your progress and eventually make suggestions on it.

- :heavy_check_mark: **DO**: Amend your commit instead of creating a new one if you forgot something minor in your last commit;

---

Thanks! :heart: :heart: :heart:
